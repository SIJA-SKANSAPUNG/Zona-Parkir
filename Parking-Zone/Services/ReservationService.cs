using Parking_Zone.Models;
using Parking_Zone.Repositories;

namespace Parking_Zone.Services
{
    public class ReservationService : Service<Reservation>, IReservationService
    {
        public ReservationService(IReservationRepository repository) : base(repository)
        {
        }

        public override void Insert(Reservation entity)
        {
            entity.Id = Guid.NewGuid();
            base.Insert(entity);
        }

        public IEnumerable<Reservation> GetByAppUserId(string appUserId)
        {
            return _repository.GetAll()
                .Where(x => x.AppUserId == appUserId);
        }

        public void Prolong(Reservation reservation, int newDuration)
        {
            reservation.Duration += newDuration;
            Update(reservation);
        }
    }
}
