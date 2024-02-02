using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingSlotService : IService<ParkingSlot>
    {
        public IEnumerable<ParkingSlot> GetByParkingZoneId(Guid parkingZoneId);
        public bool SlotExistsWithThisNumber(int slotNumber, Guid? slotId, Guid parkingZoneId);
        public IEnumerable<ParkingSlot> GetFreeByZoneIdAndTimePeriod(Guid zoneId, string startTime, int duration);
        public bool IsSlotFree(ParkingSlot slot, string startTime, int duration);
    }
}
