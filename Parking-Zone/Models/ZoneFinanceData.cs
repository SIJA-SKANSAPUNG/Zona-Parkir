using Parking_Zone.Enums;

namespace Parking_Zone.Models
{
    public class ZoneFinanceData
    {
        public Dictionary<SlotCategoryEnum, int> CategoryHours { get; set; }
    }
}
