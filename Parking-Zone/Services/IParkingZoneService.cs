using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingZoneService : IService<ParkingZone>
    {
        public List<string> GetCurrentCarsPlateNumbersByZone(ParkingZone zone);
    }
}
