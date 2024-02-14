using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IReservationService : IService<Reservation>
    {
        public IEnumerable<Reservation> GetByAppUserId(string appUserId);
        void Prolong(Reservation reservation, int extraHours);
    }
}
