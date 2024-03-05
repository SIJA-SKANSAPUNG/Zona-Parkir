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

        public HoursSummaryVM GetStandardAndBusinessHoursByPeriod(string period)
        {
            var hoursSummary = new HoursSummaryVM();
            var allReservations = _repository.GetAll();

            if (period == "all_time")
            {
                foreach (var reservation in allReservations)
                {
                    if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
                        hoursSummary.StandardHours += reservation.Duration;
                    else
                        hoursSummary.BusinessHours += reservation.Duration;
                }
            }
            if (period == "last_30_days")
            {
                var bookedReservationsLast30Days = allReservations.Where(r => r.StartTime > DateTime.Now.AddDays(-30));

                foreach (var reservation in bookedReservationsLast30Days)
                {
                    if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
                        hoursSummary.StandardHours += reservation.Duration;
                    else
                        hoursSummary.BusinessHours += reservation.Duration;
                }
            }
            if (period == "last_7_days")
            {
                var bookedReservationsLast7Days = allReservations.Where(r => r.StartTime > DateTime.Now.AddDays(-7));

                foreach (var reservation in bookedReservationsLast7Days)
                {
                    if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
                        hoursSummary.StandardHours += reservation.Duration;
                    else
                        hoursSummary.BusinessHours += reservation.Duration;
                }
            }
            if (period == "yesterday")
            {
                var bookedReservationsYesterday = allReservations.Where(r => r.StartTime.Date == DateTime.Now.AddDays(-1).Date);

                foreach (var reservation in bookedReservationsYesterday)
                {
                    if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
                        hoursSummary.StandardHours += reservation.Duration;
                    else
                        hoursSummary.BusinessHours += reservation.Duration;
                }
            }
            if (period == "today")
            {
                var bookedReservationsToday = allReservations.Where(r => r.StartTime.Date == DateTime.Now.Date);

                foreach (var reservation in bookedReservationsToday)
                {
                    if (reservation.ParkingSlot.Category == Enums.SlotCategoryEnum.Standard)
                        hoursSummary.StandardHours += reservation.Duration;
                    else
                        hoursSummary.BusinessHours += reservation.Duration;
                }
            }

            return hoursSummary;
        }
    }
}
