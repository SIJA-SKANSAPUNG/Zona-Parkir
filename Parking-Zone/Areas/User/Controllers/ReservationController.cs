using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Parking_Zone.Services;
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
            var reservationVM = new ReservationVM();

            reservationVM.ParkingZones = new SelectList(zones, "Id", "Name");

            ViewBag.HasAnySlot = false;

            return View(reservationVM);
        }

        [HttpPost]
        public IActionResult FreeSlots(ReservationVM reservationVM)
        {
            reservationVM.ParkingSlots = _slotService.GetAllSlotsByZoneIdForReservation(reservationVM.ParkingZoneId,
                                                                                        reservationVM.StartTime,
                                                                                        reservationVM.Duration);
            return View(reservationVM);
        }
    }
}
