using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingSlotService : IService<ParkingSlot>
    {
        public IEnumerable<ParkingSlot> GetByParkingZoneId(Guid parkingZoneId);
        public bool SlotExistsWithThisNumber(int slotNumber, Guid? slotId, Guid parkingZoneId);
        public IEnumerable<ParkingSlot> GetAllSlotsByZoneIdForReservation(Guid zoneId, DateTime startTime, int duration);
        public bool IsSlotFree(ParkingSlot slot, DateTime startTime, int duration);
    }
}
