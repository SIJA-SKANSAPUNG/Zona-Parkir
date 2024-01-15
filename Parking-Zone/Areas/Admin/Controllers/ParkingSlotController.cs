using Microsoft.AspNetCore.Mvc;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.ParkingZone;

namespace Parking_Zone.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var Zone = _zoneService.GetById(zoneId);

            if (Zone is null)
            {
                return BadRequest();
            }

            var slots = _slotService.GetByParkingZoneId(zoneId);

            var slotVMs = slots.Select(x => new ParkingSlotListItemVM(x)).ToList();

            ViewData["parkingZoneName"] = Zone.Name;
            ViewData["parkingZoneId"] = zoneId;

            return View(slotVMs);
        }

        // GET: Admin/ParkingSlots/Create
        public IActionResult Create(Guid zoneId)
        {
            var Zone = _zoneService.GetById(zoneId);

            if (Zone is null)
            {
                return BadRequest();
            }

            var SlotCreateVM = new ParkingSlotCreateVM();

            SlotCreateVM.ParkingZoneId = zoneId;
            SlotCreateVM.ParkingZoneName = Zone.Name;

            return View(SlotCreateVM);
        }

        // POST: Admin/ParkingSlots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ParkingSlotCreateVM slotCreateVM)
        {
            if (ModelState.IsValid)
            {
                if (_slotService.SlotExistsWithThisNumber(slotCreateVM.Number, null, slotCreateVM.ParkingZoneId))
                {
                    return BadRequest("Slot with this number already exists");
                }

                var slot = slotCreateVM.MapToModel();
                _slotService.Insert(slot);
                return RedirectToAction("Index", "ParkingSlot", new { parkingZoneId = slotCreateVM.ParkingZoneId });
            }
            return View(slotCreateVM);
        }
    }
}
