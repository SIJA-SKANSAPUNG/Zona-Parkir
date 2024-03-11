using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parking_Zone.Enums;
using Parking_Zone.Services;
using Parking_Zone.Services.Models;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.ParkingZone;
using System.Text.Json;

namespace Parking_Zone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ParkingSlotController : Controller
    {
        private readonly IParkingSlotService _slotService;
        private readonly IParkingZoneService _zoneService;

        public ParkingSlotController(IParkingZoneService zoneService, IParkingSlotService slotService)
        {
            _slotService = slotService;
            _zoneService = zoneService;
        }

        public IActionResult Index(Guid zoneId)
        {
            var zone = _zoneService.GetById(zoneId);

            if (zone is null)
            {
                return BadRequest();
            }
            ViewData["parkingZoneName"] = zone.Name;
            ViewData["parkingZoneId"] = zoneId;

            return View();
        }

        [HttpPost]
        public IActionResult LoadData(FilterSlotVM slotVM)
        {
            var zone = _zoneService.GetById(slotVM.ZoneId);

            if (zone is null)
                return BadRequest("Zone Not Found");

            var filterSlotsQuery = new FilterSlotsQuery()
            {
                ZoneId = slotVM.ZoneId,
                OnlyFree = slotVM.OnlyFree,
                Category = slotVM.Category
            };
            var slots = _slotService.Filter(filterSlotsQuery);
            var slotVMs = slots.Select(s => new ParkingSlotListItemVM(s)).ToList();

            return Json(slotVMs);
        }

        // GET: Admin/ParkingSlots/Create
        public IActionResult Create(Guid zoneId)
        {
            var zone = _zoneService.GetById(zoneId);

            if (zone is null)
            {
                return BadRequest();
            }

            var slotCreateVM = new ParkingSlotCreateVM();

            slotCreateVM.ParkingZoneId = zoneId;
            slotCreateVM.ParkingZoneName = zone.Name;

            return View(slotCreateVM);
        }

        // POST: Admin/ParkingSlots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ParkingSlotCreateVM slotCreateVM)
        {
            if (_slotService.SlotExistsWithThisNumber(slotCreateVM.Number, null, slotCreateVM.ParkingZoneId))
            {
                ModelState.AddModelError("Number", "This Slot number already exists");
            }

            if (ModelState.IsValid)
            {
                var zone = _zoneService.GetById(slotCreateVM.ParkingZoneId);

                if (zone is null)
                    return BadRequest("Parking Zone Not Found");

                var slot = slotCreateVM.MapToModel();
                _slotService.Insert(slot);
                return RedirectToAction("Index", "ParkingSlot", new { zoneId = slotCreateVM.ParkingZoneId });
            }
            return View(slotCreateVM);
        }

        // GET: Admin/ParkingSlots/Edit/5
        public IActionResult Edit(Guid id)
        {
            var parkingSlot = _slotService.GetById(id);

            if (parkingSlot == null)
            {
                return NotFound();
            }

            var parkingSlotEditVM = new ParkingSlotEditVM(parkingSlot);

            return View(parkingSlotEditVM);
        }

        // POST: Admin/ParkingSlots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, ParkingSlotEditVM parkingSlotEditVM)
        {
            if (id != parkingSlotEditVM.Id)
            {
                return NotFound();
            }

            if (_slotService.SlotExistsWithThisNumber(parkingSlotEditVM.Number, parkingSlotEditVM.Id, parkingSlotEditVM.ParkingZoneId))
            {
                ModelState.AddModelError("Number", "This Slot number already exists");
            }

            var existingSlot = _slotService.GetById(parkingSlotEditVM.Id);

            if (existingSlot == null)
            {
                return NotFound();
            }

            if (existingSlot.HasAnyActiveReservation == true && existingSlot.Category != parkingSlotEditVM.Category)
            {
                ModelState.AddModelError("Category", "Category cannot be changed it is in use");
            }

            if (ModelState.IsValid)
            {
                var slotVM = parkingSlotEditVM.MapToModel(existingSlot);
                _slotService.Update(slotVM);

                return RedirectToAction("Index", "ParkingSlot", new { zoneId = parkingSlotEditVM.ParkingZoneId });
            }
            return View(parkingSlotEditVM);
        }

        // GET: Admin/ParkingSlots/Details/5
        public IActionResult Details(Guid id)
        {
            var slot = _slotService.GetById(id);

            if (slot is null)
                return NotFound();

            var slotVM = new ParkingSlotDetailsVM(slot);

            return View(slotVM);
        }

        // GET: Admin/ParkingSlots/Delete/5
        public IActionResult Delete(Guid id)
        {
            var slot = _slotService.GetById(id);

            if (slot == null)
            {
                return NotFound();
            }
            var slotVM = new ParkingSlotDetailsVM(slot);

            return View(slotVM);
        }

        // POST: Admin/ParkingSlots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id, Guid parkingZoneId)
        {
            var parkingSlot = _slotService.GetById(id);

            if (parkingSlot == null)
            {
                return NotFound();
            }
            if (parkingSlot.HasAnyActiveReservation)
            {
                return BadRequest();
            }
            _slotService.Delete(parkingSlot);

            return RedirectToAction("Index", "ParkingSlot", new { zoneId = parkingZoneId });
        }
    }
}
