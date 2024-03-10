using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Enums
{
    public enum PeriodOptionsEnum
    {
        Today = 1,
        Yesterday,
        [Display(Name = "Last 7 Days")]
        Last7Days,
        [Display(Name = "Last 30 Days")]
        Last30Days,
        [Display(Name = "All Time")]
        AllTime
    }
}
