using Microsoft.AspNetCore.Mvc;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.ParkingZone;

namespace Parking_Zone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ParkingSlotController : Controller
    {
        private readonly IParkingSlotService _parkingSlotService;
        private readonly IParkingZoneService _parkingZoneService;

        public ParkingSlotController(IParkingZoneService parkingZoneService, IParkingSlotService parkingSlotService)
        {
            _parkingSlotService = parkingSlotService;
            _parkingZoneService = parkingZoneService;
        }

        public IActionResult Index(Guid parkingZoneId)
        {
            var parkingZone = _parkingZoneService.GetById(parkingZoneId);

            if (parkingZone is null)
            {
                return BadRequest();
            }

            var slots = _parkingSlotService.GetByParkingZoneId(parkingZoneId);

            var slotVMs = slots.Select(x => new ParkingSlotListItemVM(x)).ToList();

            ViewData["parkingZoneName"] = parkingZone.Name;
            ViewData["parkingZoneId"] = parkingZoneId;

            return View(slotVMs);
        }

        // GET: Admin/ParkingSlots/Create
        public IActionResult Create(Guid parkingZoneId)
        {
            var parkingZone = _parkingZoneService.GetById(parkingZoneId);

            if (parkingZone is null)
            {
                return BadRequest();
            }

            var parkingSlotCreateVM = new ParkingSlotCreateVM();

            parkingSlotCreateVM.ParkingZoneId = parkingZoneId;
            parkingSlotCreateVM.ParkingZoneName = parkingZone.Name;

            return View(parkingSlotCreateVM);
        }

        // POST: Admin/ParkingSlots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ParkingSlotCreateVM parkingSlotCreateVM)
        {
            if (ModelState.IsValid)
            {
                if (_parkingSlotService.SlotExistsWithThisNumber(parkingSlotCreateVM.Number, null, parkingSlotCreateVM.ParkingZoneId))
                {
                    return BadRequest("Slot with this number already exists");
                }

                var slot = parkingSlotCreateVM.MapToModel();
                _parkingSlotService.Insert(slot);
                return RedirectToAction("Index", "ParkingSlot", new { parkingZoneId = parkingSlotCreateVM.ParkingZoneId });
            }
            return View(parkingSlotCreateVM);
        }
    }
}
