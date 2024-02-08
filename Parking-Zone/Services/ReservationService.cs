using Parking_Zone.Models;
using Parking_Zone.Repositories;

namespace Parking_Zone.Services
{
    public class ReservationService : Service<Reservation>, IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        public ReservationService(IReservationRepository repository) : base(repository)
        {
            _reservationRepository = repository;
        }

        public override void Insert(Reservation entity)
        {
            entity.Id = Guid.NewGuid();
            base.Insert(entity);
        }

        public IEnumerable<Reservation> GetByAppUserId(string appUserId)
        {
            return _reservationRepository.GetAll()
                .Where(x => x.AppUserId == appUserId);
        }
    }
}
