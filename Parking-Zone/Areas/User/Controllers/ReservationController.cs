using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Parking_Zone.Models;
using Parking_Zone.Services;
using Parking_Zone.ViewModels.ParkingSlot;
using Parking_Zone.ViewModels.Reservation;
using System.Security.Claims;

namespace Parking_Zone.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IParkingZoneService _zoneService;
        private readonly IParkingSlotService _slotService;
        private readonly IReservationService _reservationService;

        public ReservationController(IParkingZoneService zoneService,
            IParkingSlotService slotService,
            IReservationService reservationService)
        {
            _zoneService = zoneService;
            _slotService = slotService;
            _reservationService = reservationService;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reservations = _reservationService.GetByAppUserId(userId);

            var reservationVMs = reservations
                .Select(x => new ReservationListItemVM(x))
                .OrderByDescending(x => x.StartTime);

            return View(reservationVMs);
        }

        public IActionResult Prolong(Guid reservationId)
        {
            var reservation = _reservationService.GetById(reservationId);
            if (reservation is null)
            {
                return NotFound();
            }

            var prolongVM = new ProlongVM(reservation);
            return View(prolongVM);
        }

        [HttpPost]
        public IActionResult Prolong(ProlongVM prolongVM)
        {
            var reservation = _reservationService.GetById(prolongVM.ReservationId);
            if (reservation is null)
            {
                return NotFound();
            }

            if (reservation.StartTime > DateTime.Now ||
                reservation.StartTime.AddHours(reservation.Duration) < DateTime.Now)
            {
                ModelState.AddModelError("", "This Slot Not active at the moment");
                return View(prolongVM);
            }
            if (!_slotService.IsSlotFree(reservation.ParkingSlot, 
                DateTime.Parse(prolongVM.EndDateTime), 
                prolongVM.NewDuration))
            {
                ModelState.AddModelError("NewDuration", "In this time another reservation booked, try another time");
                return View(prolongVM);
            }

            _reservationService.Prolong(reservation, prolongVM.NewDuration);

            return RedirectToAction("Index", "Reservation", new { area = "User" });
        }
    }
}