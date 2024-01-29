using Microsoft.AspNetCore.Mvc;

namespace Parking_Zone.Areas.User.Controllers
{
    [Area("User")]
    public class ReservationController : Controller
    {
        public ReservationController()
        {
            
        }

        public IActionResult FreeSlots()
        {
            return View();
        }
    }
}
