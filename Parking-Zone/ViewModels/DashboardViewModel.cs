using System;
using System.Collections.Generic;

namespace Parking_Zone.ViewModels
{
    public class DashboardViewModel
    {
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new List<RecentActivityViewModel>();
        public List<GateStatus> Gates { get; set; } = new List<GateStatus>();
        public decimal OccupancyRate { get; set; }
        public List<HourlyOccupancyData> HourlyOccupancy { get; set; } = new List<HourlyOccupancyData>();
        public Dictionary<string, int> VehicleDistribution { get; set; } = new Dictionary<string, int>();

        public class RecentActivityViewModel
        {
            public DateTime Timestamp { get; set; }
            public string LicensePlate { get; set; } = null!;
            public string VehicleType { get; set; } = null!;
            public string ActionType { get; set; } = null!;
            public decimal Fee { get; set; }
        }

        public class GateStatus
        {
            public string Id { get; set; } = null!;
            public bool IsOpen { get; set; }
        }

        public class HourlyOccupancyData
        {
            public int Hour { get; set; }
            public int OccupancyPercentage { get; set; }
        }
    }
}
