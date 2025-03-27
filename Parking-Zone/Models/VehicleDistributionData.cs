using System;
using System.Collections.Generic;

namespace Parking_Zone.Models
{
    public class VehicleDistributionData
    {
        public DateTime Timestamp { get; set; }
        public Dictionary<string, int> VehicleTypeDistribution { get; set; }
        public Dictionary<string, decimal> RevenueByVehicleType { get; set; }
        public int TotalVehicles { get; set; }
        public decimal TotalRevenue { get; set; }

        public VehicleDistributionData()
        {
            VehicleTypeDistribution = new Dictionary<string, int>();
            RevenueByVehicleType = new Dictionary<string, decimal>();
        }
    }
} 