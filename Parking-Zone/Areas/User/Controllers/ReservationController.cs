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
        private readonly IReservationService _reservationService;
        public ReservationController(IParkingZoneService zoneService, IParkingSlotService slotService, IReservationService reservationService)
        {
            _zoneService = zoneService;
            _slotService = slotService;
            _reservationService = reservationService;
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

        public IActionResult Reserve(Guid SlotId, string startTime, int duration)
        {
            var slot = _slotService.GetById(SlotId);

            var reserveVM = new ReserveVM(slot, startTime, duration);

            return View(reserveVM);
        }

        [HttpPost]
        public IActionResult Reserve(ReserveVM reserveVM)
        {
            var slot = _slotService.GetById(reserveVM.SlotId);

            if (ModelState.IsValid)
            {
                if (_slotService.IsSlotFree(slot, DateTime.Parse(reserveVM.StartTime), reserveVM.Duration))
                {
                    var reservation = reserveVM.MapToModel();
                    _reservationService.Insert(reservation);

                    ViewBag.SuccessMessage = "Reservation created successfully.";

                    return View(reserveVM);
                }
            }
            return BadRequest();
        }
    }
}
