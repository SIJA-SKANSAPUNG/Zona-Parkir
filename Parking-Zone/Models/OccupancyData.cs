using System;

namespace Parking_Zone.Models
{
    public class OccupancyData
    {
        public DateTime Timestamp { get; set; }
        public int TotalSpaces { get; set; }
        public int OccupiedSpaces { get; set; }
        public int AvailableSpaces { get; set; }
        public decimal OccupancyRate { get; set; }
        public int ReservedSpaces { get; set; }
        public int DisabledSpaces { get; set; }
    }
} 