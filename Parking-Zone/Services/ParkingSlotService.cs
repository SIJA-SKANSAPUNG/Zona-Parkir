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

        public IEnumerable<ParkingSlot> GetByParkingZoneId(Guid parkingZoneId)
        {
            return _parkingSlotRepository.GetAll().Where(x => x.ParkingZoneId == parkingZoneId);
        }

        public override void Insert(ParkingSlot slot)
        {
            slot.Id = Guid.NewGuid();   
            base.Insert(slot);
        }

        public bool SlotExistsWithThisNumber(int slotNumber, Guid? slotId, Guid parkingZoneId)
            => _parkingSlotRepository.GetAll()
                .Where(s => s.ParkingZoneId == parkingZoneId && s.Id != slotId)
                .FirstOrDefault(s => s.Number == slotNumber) != null;

        public IEnumerable<ParkingSlot> GetAllSlotsByZoneIdForReservation(Guid zoneId, DateTime startTime, int duration)
            => _parkingSlotRepository.GetAll()
                .Where(x => x.ParkingZoneId == zoneId && x.IsAvailableForBooking && IsSlotFree(x, startTime, duration));

        public bool IsSlotFree(ParkingSlot slot, DateTime startTime, int duration)
        {
            var reservations = slot.Reservations.ToList();

            foreach (var reservation in reservations)
            {
                if (reservation.StartTime > startTime)
                {
                    if (startTime.AddHours(duration) > reservation.StartTime)
                        return false;
                }
                else
                {
                    if (reservation.StartTime.AddHours(reservation.Duration) > startTime)
                        return false;
                }
            }
            return true;
        }
    }
}
