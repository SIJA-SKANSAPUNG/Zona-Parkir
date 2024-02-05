using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.Reservation;

namespace Parking_Zone.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IParkingZoneService _zoneService;
        private readonly IParkingSlotService _slotService;
        private readonly IReservationService _reservationService;
        private readonly UserManager<AppUser> _userManager;

        public ReservationController(IParkingZoneService zoneService,
            IParkingSlotService slotService,
            IReservationService reservationService,
            UserManager<AppUser> userManager)
        {
            _zoneService = zoneService;
            _slotService = slotService;
            _reservationService = reservationService;
            _userManager = userManager;
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
                reservation.AppUserId = _userManager.GetUserId(User);

                _reservationService.Insert(reservation);

                ViewBag.SuccessMessage = "Reservation created successfully.";

                return View(reserveVM);
            }
            else
            {
                ModelState.AddModelError("StartTime", "Choose other time");
                ModelState.AddModelError("Duration", "This slot has just been booked, try another time");
            }

            return View(reserveVM);
        }
    }
}
