using Parking_Zone.Enums;

namespace Parking_Zone.Helpers
{
    public class ParkingZoneHelper
    {
        public static (DateTime StartInclusive, DateTime EndExclusive) BuildPeriod(PeriodOptionsEnum periodOption)
        {
            var periodStartInclusive = DateTime.MinValue;
            var periodEndExclusive = DateTime.MinValue;
            var Now = DateTime.Now;

            (periodStartInclusive, periodEndExclusive) = periodOption switch
            {
                PeriodOptionsEnum.Today => (Now.Date, Now.Date),
                PeriodOptionsEnum.Yesterday => (Now.AddDays(-1).Date, Now.AddDays(-1).Date),
                PeriodOptionsEnum.Last7Days => (Now.AddDays(-7).Date, Now.Date),
                PeriodOptionsEnum.Last30Days => (Now.AddDays(-30).Date, Now.Date),
                _ => (DateTime.MinValue, DateTime.MaxValue)
            };

            return (periodStartInclusive, periodEndExclusive);
        }
    }
}
