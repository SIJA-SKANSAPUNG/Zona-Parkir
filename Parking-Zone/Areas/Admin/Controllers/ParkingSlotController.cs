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
            var slots = _parkingSlotService.GetByParkingZoneId(parkingZoneId);

            var slotVMs = slots.Select(x => new ParkingSlotListItemVM(x)).ToList();

            var parkingZone = _parkingZoneService.GetById(parkingZoneId);

            if (parkingZone is null)
            {
                return BadRequest();
            }

            ViewData["parkingZoneName"] = parkingZone.Name;
            ViewData["parkingZoneId"] = parkingZoneId;

            return View(slotVMs);
        }
    }
}
