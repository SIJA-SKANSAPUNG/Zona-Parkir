using System;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class SystemStatusViewModel
    {
        // Parking Statistics
        public int TotalParkingSpaces { get; set; }
        public int AvailableSpaces { get; set; }
        public int OccupiedSpaces { get; set; }
        public decimal OccupancyRate { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodayVehicleCount { get; set; }

        // Gate Status
        public List<GateStatusInfo> Gates { get; set; }

        // Hardware Status
        public List<HardwareStatusInfo> Cameras { get; set; }
        public List<HardwareStatusInfo> Printers { get; set; }
        public List<HardwareStatusInfo> Scanners { get; set; }

        // Recent Activity
        public List<RecentActivityInfo> RecentActivities { get; set; }

        public SystemStatusViewModel()
        {
            Gates = new List<GateStatusInfo>();
            Cameras = new List<HardwareStatusInfo>();
            Printers = new List<HardwareStatusInfo>();
            Scanners = new List<HardwareStatusInfo>();
            RecentActivities = new List<RecentActivityInfo>();
        }
    }

    public class GateStatusInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Type { get; set; } // Entry, Exit, Both
        public string Status { get; set; } // Online, Offline, Maintenance
        public DateTime? LastActivity { get; set; }
        public string CurrentOperator { get; set; }
    }

    public class HardwareStatusInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastChecked { get; set; }
        public string Status { get; set; }
        public string AssignedGate { get; set; }
    }

    public class RecentActivityInfo
    {
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; } // Entry, Exit, Payment, etc.
        public string Description { get; set; }
        public string Location { get; set; }
        public string Operator { get; set; }
        public decimal? Amount { get; set; }
        public string VehicleInfo { get; set; }
    }
}