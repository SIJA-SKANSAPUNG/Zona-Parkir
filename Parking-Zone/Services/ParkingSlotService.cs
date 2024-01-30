using Parking_Zone.Models;
using Parking_Zone.Repositories;
using System.Diagnostics.Eventing.Reader;

namespace Parking_Zone.Services
{
    public class ParkingSlotService : Service<ParkingSlot>, IParkingSlotService
    {
        private IParkingSlotRepository _parkingSlotRepository;

        public ParkingSlotService(IParkingSlotRepository repository) : base(repository)
        {
            _parkingSlotRepository = repository;
        }

        public override void Insert(ParkingSlot slot)
        {
            slot.Id = Guid.NewGuid();
            base.Insert(slot);
        }

        public IEnumerable<ParkingSlot> GetByParkingZoneId(Guid parkingZoneId)
            => _parkingSlotRepository.GetAll().Where(x => x.ParkingZoneId == parkingZoneId);

        public bool SlotExistsWithThisNumber(int slotNumber, Guid? slotId, Guid parkingZoneId)
            => _parkingSlotRepository.GetAll()
                .Where(s => s.ParkingZoneId == parkingZoneId && s.Id != slotId)
                .FirstOrDefault(s => s.Number == slotNumber) != null;

        public IEnumerable<ParkingSlot> GetFreeByZoneIdAndTimePeriod(Guid zoneId, DateTime startTime, int duration)
            => _parkingSlotRepository.GetAllWithReservations()
                .Where(x => x.ParkingZoneId == zoneId && x.IsAvailableForBooking && IsSlotFree(x, startTime, duration));

        public bool IsSlotFree(ParkingSlot slot, DateTime startTime, int duration)
            => slot.Reservations.All(reservation =>
                !(reservation.StartTime <= startTime && startTime < reservation.StartTime.AddHours(reservation.Duration)) &&
                !(reservation.StartTime > startTime && startTime.AddHours(duration) > reservation.StartTime));
    }
}
