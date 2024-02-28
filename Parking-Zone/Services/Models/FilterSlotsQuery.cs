using Parking_Zone.Enums;
using Parking_Zone.ViewModels.ParkingSlot;

namespace Parking_Zone.Services.Models
{
    public class FilterSlotsQuery
    {
        public Guid ZoneId { get; set; }
        public bool OnlyFree { get; set; }
        public SlotCategoryEnum Category { get; set; }
    }
}
