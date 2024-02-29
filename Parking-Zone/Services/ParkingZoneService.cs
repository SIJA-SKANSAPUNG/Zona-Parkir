using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Repositories;

namespace Parking_Zone.Services
{
    public class ParkingZoneService : Service<ParkingZone>, IParkingZoneService
    {
        public ParkingZoneService(IParkingZoneRepository repository) : base(repository)
        {

        }
        public override void Insert(ParkingZone parkingZone)
        {
            parkingZone.Id = Guid.NewGuid();
            parkingZone.DateOfEstablishment = DateTime.UtcNow;
            base.Insert(parkingZone);
        }
        public List<string> GetCurrentCarsPlateNumbersByZone(ParkingZone zone)
        {
            return zone.ParkingSlots
                .SelectMany(slot => slot.Reservations
                    .Where(reservation => reservation.IsOnGoing)
                    .Select(reservation => reservation.VehicleNumber))
                .ToList();
        }
    }
}
