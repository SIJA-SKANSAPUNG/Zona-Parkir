using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
