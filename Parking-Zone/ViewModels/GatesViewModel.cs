using System;
using System.Collections.Generic;

namespace Parking_Zone.ViewModels
{
    public class GatesViewModel
    {
        public List<EntryGateViewModel> EntryGates { get; set; } = new List<EntryGateViewModel>();
        public List<ExitGateViewModel> ExitGates { get; set; } = new List<ExitGateViewModel>();
    }

    public class ExitGateViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public DateTime? LastActivity { get; set; }
        public int TodayExitCount { get; set; }
    }

    public class GateOperationViewModel
    {
        public Guid? GateId { get; set; }
        public string GateType { get; set; } // "Entry" or "Exit"
        public string OperationType { get; set; } // "Open", "Close", etc.
        public string Status { get; set; } // "Success", "Failure"
        public string Details { get; set; }
        public string TransactionId { get; set; }
        public Guid? OperatorId { get; set; }
        public string OperatorName { get; set; }
    }
} 