using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Services
{
    public interface IReservationService : IService<Reservation>
    {
        public IEnumerable<Reservation> GetByAppUserId(string appUserId);
        void Prolong(Reservation reservation, int extraHours);
        public ReservationHoursSummary GetStandardAndBusinessHoursByPeriod(PeriodsEnum period, ParkingZone zone);
    }
}
