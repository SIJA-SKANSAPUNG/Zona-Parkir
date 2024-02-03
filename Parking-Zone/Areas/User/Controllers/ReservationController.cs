using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.Reservation;

namespace Parking_Zone.Areas.User.Controllers
{
    [Area("User")]
    public class ReservationController : Controller
    {
        private readonly IParkingZoneService _zoneService;
        private readonly IParkingSlotService _slotService;

        public ReservationController(IParkingZoneService zoneService, IParkingSlotService slotService)
        {
            _zoneService = zoneService;
            _slotService = slotService;
        }

        public IActionResult FreeSlots()
        {
            var zones = _zoneService.GetAll();

            FreeSlotsVM freeSlotsVM = new FreeSlotsVM(zones);

            return View(freeSlotsVM);
        }

        [HttpPost]
        public IActionResult FreeSlots(FreeSlotsVM freeSlotsVM)
        {
            freeSlotsVM.ParkingSlots = _slotService
                .GetFreeByZoneIdAndTimePeriod(freeSlotsVM.ParkingZoneId, DateTime.Parse(freeSlotsVM.StartTime), freeSlotsVM.Duration)
                .Select(x => new ParkingSlotListItemVM(x));

            var zones = _zoneService.GetAll();

            freeSlotsVM.ParkingZones = new SelectList(zones, "Id", "Name");

            return View(freeSlotsVM);
        }
    }
}
