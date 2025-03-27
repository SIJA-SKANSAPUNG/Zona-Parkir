using System;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class DashboardParkingActivity
    {
        public Guid Id { get; set; }
        public string VehicleType { get; set; }
        public string LicensePlate { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } // "Entry" or "Exit"
        public decimal Fee { get; set; }
        public string ParkingType { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class OccupancyData
    {
        public int Hour { get; set; }
        public int Count { get; set; }
        public decimal OccupancyPercentage { get; set; }
    }

    public class VehicleDistributionData
    {
        public string Type { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    public class OccupancyDataComparer : IEqualityComparer<OccupancyData>
    {
        public bool Equals(OccupancyData x, OccupancyData y)
        {
            return x.Hour == y.Hour;
        }

        public int GetHashCode(OccupancyData obj)
        {
            return obj.Hour.GetHashCode();
        }
    }

    public class DashboardViewModel
    {
        public int TotalSpaces { get; set; }
        public int OccupiedSpaces { get; set; }
        public decimal DailyRevenue { get; set; }
        public double OccupancyRate => TotalSpaces > 0 ? (double)OccupiedSpaces / TotalSpaces * 100 : 0;

        public List<VehicleTypeStats> VehicleTypeDistribution { get; set; } = new List<VehicleTypeStats>();
        public List<int> HourlyOccupancy { get; set; } = new List<int>();

        public int TotalVehiclesToday { get; set; }
        public int TotalVehiclesThisMonth { get; set; }

        public int AvailableSpaces => TotalSpaces - OccupiedSpaces;
        public List<RecentActivityViewModel> RecentActivity { get; set; } = new List<RecentActivityViewModel>();
        public List<VehicleDistributionData> VehicleDistribution { get; set; } = new List<VehicleDistributionData>();

        public List<GateViewModel> Gates { get; set; } = new List<GateViewModel>();

        public class GateViewModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool IsOperational { get; set; }
            public int TransactionsToday { get; set; }
            public bool IsOpen { get; set; } = false;
        }

        public class RecentActivityViewModel
        {
            public string Action { get; set; }
            public DateTime Timestamp { get; set; }
            public string Details { get; set; }
        }
    }
}