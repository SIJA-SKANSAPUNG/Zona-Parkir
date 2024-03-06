using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Services
{
    public class ReservationService : Service<Reservation>, IReservationService
    {
        public ReservationService(IReservationRepository repository) : base(repository)
        {
        }

        public override void Insert(Reservation entity)
        {
            entity.Id = Guid.NewGuid();
            base.Insert(entity);
        }

        public IEnumerable<Reservation> GetByAppUserId(string appUserId)
        {
            return _repository.GetAll()
                .Where(x => x.AppUserId == appUserId);
        }

        public void Prolong(Reservation reservation, int extraHours)
        {
            reservation.Duration += extraHours;
            Update(reservation);
        }

        public ReservationHoursSummaryVM GetStandardAndBusinessHoursByPeriod(PeriodsEnum period, ParkingZone zone)
        {
            var reservations = zone.ParkingSlots.SelectMany(s => s.Reservations);
            var hoursSummary = new ReservationHoursSummaryVM();
            var targetDate = DateTime.Now;

            var reservationsForPeriod = period switch
            {
                PeriodsEnum.Today => reservations.Where(r => r.StartTime.Date == targetDate.Date),
                PeriodsEnum.Yesterday => reservations.Where(r => r.StartTime.Date == targetDate.AddDays(-1).Date),
                PeriodsEnum.Last7Days => reservations.Where(r => r.StartTime >= targetDate.AddDays(-7) && r.StartTime <= targetDate),
                PeriodsEnum.Last30Days => reservations.Where(r => r.StartTime >= targetDate.AddDays(-30) && r.StartTime <= targetDate),
                _=>  reservations
            };

            foreach (var reservation in reservationsForPeriod)
            {
                if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
                    hoursSummary.StandardHours += reservation.Duration;
                else
                    hoursSummary.BusinessHours += reservation.Duration;
            }

            return hoursSummary;
        }
    }
}
