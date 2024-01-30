using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Repositories
{
    public class ParkingSlotRepository : Repository<ParkingSlot>, IParkingSlotRepository
    {
        private readonly ApplicationDbContext _context;
        public ParkingSlotRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<ParkingSlot> GetAllWithReservations()
            => _context.ParkingSlots.Include(x => x.Reservations);
    }
}
