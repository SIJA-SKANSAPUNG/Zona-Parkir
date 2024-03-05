using Microsoft.AspNetCore.Mvc;
using Parking_Zone.Services;

namespace Parking_Zone.Areas.Admin.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public IActionResult Index(string period)
        {
            var reservations = _reservationService.GetAll();
            var response = _reservationService.GetStandardAndBusinessHoursByPeriod(reservations, period);
            return Json(response);
        }
    }
}
