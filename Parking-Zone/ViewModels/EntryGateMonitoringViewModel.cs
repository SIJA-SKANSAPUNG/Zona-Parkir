using System;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class EntryGateMonitoringViewModel
    {
        public List<EntryGateStatusViewModel> Gates { get; set; } = new List<EntryGateStatusViewModel>();
        public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new List<RecentTransactionViewModel>();
    }
    
    public class EntryGateStatusViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastActivity { get; set; }
        public int TransactionsToday { get; set; }
        public string Status { get; set; }
    }
    
    public class RecentTransactionViewModel
    {
        public string TicketNumber { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime EntryTime { get; set; }
        public string EntryPoint { get; set; }
        public string VehicleType { get; set; }
        public string Status { get; set; }
    }
}
