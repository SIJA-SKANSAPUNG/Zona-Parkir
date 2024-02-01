using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Repositories
{
    public class ParkingSlotRepository : Repository<ParkingSlot>, IParkingSlotRepository
    {
        public ParkingSlotRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<ParkingSlot> GetAllWithReservations()
            => entities.Include(x => x.Reservations);
    }
}
