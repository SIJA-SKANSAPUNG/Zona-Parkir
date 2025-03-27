using System;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class EntryGateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public DateTime? LastActivity { get; set; }
        public int TodayEntryCount { get; set; }
        public List<ParkingTransaction> RecentTransactions { get; set; } = new List<ParkingTransaction>();
        public bool IsOfflineMode { get; set; }
        public DateTime LastSync { get; set; }
        public List<VehicleEntry> RecentEntries { get; set; } = new List<VehicleEntry>();
        public bool IsCameraActive { get; set; }
        public bool IsPrinterActive { get; set; }
        public string OperatorName { get; set; }
        public Guid GateId { get; set; }
    }
}
