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
    }
}
