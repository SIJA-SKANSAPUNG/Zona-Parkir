﻿using Parking_Zone.Enums;

namespace Parking_Zone.ViewModels.ParkingSlot
{
    public class FilterSlotVM
    {
        public Guid ZoneId { get; set; }
        public bool OnlyFree { get; set; }
        public SlotCategoryEnum Category { get; set; }
    }
}
