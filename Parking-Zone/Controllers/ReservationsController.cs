using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.Reservation;
using System.Security.Claims;

namespace Parking_Zone.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IParkingZoneService _zoneService;
        private readonly IParkingSlotService _slotService;
        private readonly IReservationService _reservationService;

        public ReservationsController(IParkingZoneService zoneService,
            IParkingSlotService slotService,
            IReservationService reservationService)
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

            if (slot is null)
                return NotFound();

            var reserveVM = new ReserveVM(slot, startTime, duration);

            return View(reserveVM);
        }

        [HttpPost]
        public IActionResult Reserve(ReserveVM reserveVM)
        {
            var slot = _slotService.GetById(reserveVM.SlotId);

            if (slot is null)
                return NotFound();

            reserveVM.SlotNumber = slot.Number;
            reserveVM.ZoneAddress = slot.ParkingZone.Address;
            reserveVM.ZoneName = slot.ParkingZone.Name;

            if (_slotService.IsSlotFree(slot, DateTime.Parse(reserveVM.StartTime), reserveVM.Duration))
            {
                var reservation = reserveVM.MapToModel();

                reservation.AppUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _reservationService.Insert(reservation);
            }
            else
            {
                ModelState.AddModelError("StartTime", "Choose other time");
                ModelState.AddModelError("Duration", "This slot has just been booked, try another time");
            }

            return RedirectToAction("Index", "Reservation", new { area = "User" });
        }
    }
}
