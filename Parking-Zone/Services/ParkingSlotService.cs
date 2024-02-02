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

        public IEnumerable<ParkingSlot> GetFreeByZoneIdAndTimePeriod(Guid zoneId, string startTime, int duration)
            => _parkingSlotRepository.GetAllWithReservations()
                .Where(x => x.ParkingZoneId == zoneId && x.IsAvailableForBooking && IsSlotFree(x, startTime, duration));

        public bool IsSlotFree(ParkingSlot slot, string startTime, int duration)
        {
            var start_time = DateTime.Parse(startTime);
            
            foreach(var reservation in slot.Reservations)
            {
                var reservation_start_time = DateTime.Parse(reservation.StartTime);

                if (reservation_start_time <= start_time && start_time < reservation_start_time.AddHours(reservation.Duration))
                {
                    return false;
                }
                else if(reservation_start_time > start_time && start_time.AddHours(reservation.Duration) > reservation_start_time)
                {
                    return false;
                }
            }
            return true;

            //=> !slot.Reservations.Any(reservation =>
            //    reservation.StartTime <= startTime && startTime < reservation.StartTime.AddHours(reservation.Duration) ||
            //    reservation.StartTime > startTime && startTime.AddHours(duration) > reservation.StartTime);
        }
    }
}