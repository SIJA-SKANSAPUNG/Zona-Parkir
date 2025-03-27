using System;

namespace Parking_Zone.Models
{
    public class VehicleTypeStats
    {
        public string VehicleType { get; set; }
        public int Count { get; set; }
        public decimal TotalRevenue { get; set; }
        public double Percentage { get; set; }
    }
}
