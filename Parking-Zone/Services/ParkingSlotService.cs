﻿using NuGet.Protocol;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services.Models;
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
            => !slot.Reservations.Any(reservation =>
                reservation.StartTime <= startTime && startTime < reservation.StartTime.AddHours(reservation.Duration) ||
                reservation.StartTime > startTime && startTime.AddHours(duration) > reservation.StartTime);

        public IEnumerable<ParkingSlot> Filter(FilterSlotsQuery slotFilterQuery)
        {
            var slots = GetByParkingZoneId(slotFilterQuery.ZoneId);

            if (slotFilterQuery.OnlyFree)
            {
                slots = slots.Where(s => !s.HasAnyActiveReservation);
            }
            if (slotFilterQuery.Category != 0)
            {
                slots = slots.Where(s => s.Category == slotFilterQuery.Category);
            }

            return slots;
        }
    }
}