using Parking_Zone.Models;

namespace Parking_Zone.Repositories
{
    public interface IParkingSlotRepository : IRepository<ParkingSlot>
    {
        public IEnumerable<ParkingSlot> GetAllWithReservations();
    }
}
