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

        public ZoneFinanceData GetZoneFinanceDataByPeriod(DateTime startInclusive, DateTime endExclusive, ParkingZone zone)
        {
            var zoneFinanceData = new ZoneFinanceData();
            var slots = zone.ParkingSlots;

            zoneFinanceData.CategoryHours = slots
                .GroupBy(s => s.Category)
                .ToDictionary(
                    g => g.Key,
                    g => g.SelectMany(r => r.Reservations
                                                            .Where(r => r.StartTime >= startInclusive && r.StartTime < endExclusive))
                                                            .Sum(r => r.Duration));

            //foreach (var reservation in reservationsForPeriod)
            //{
            //    if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
            //        hoursSummary.StandardHours += reservation.Duration;
            //    else
            //        hoursSummary.BusinessHours += reservation.Duration;
            //}

            return zoneFinanceData;
        }
    }
}
