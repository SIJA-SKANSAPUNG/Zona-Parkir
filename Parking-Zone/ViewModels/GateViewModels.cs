using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Parking_Zone.Models;

namespace Parking_Zone.ViewModels
{
    public class GateOperationalViewModel
    {
        [Display(Name = "Gate ID")]
        public string GateId { get; set; } = "GATE-01";

        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Ready";

        [Display(Name = "Camera Active")]
        public bool IsCameraActive { get; set; } = false;

        [Display(Name = "Printer Active")]
        public bool IsPrinterActive { get; set; } = true;

        [Display(Name = "Offline Mode")]
        public bool IsOfflineMode { get; set; } = false;

        [Display(Name = "Last Sync")]
        public DateTime LastSync { get; set; } = DateTime.Now;

        public List<VehicleEntry> RecentEntries { get; set; } = new();
    }

    public class GateExitOperationViewModel
    {
        [Display(Name = "Gate ID")]
        public string GateId { get; set; } = "GATE-02";

        [Display(Name = "Operator")]
        public string OperatorName { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Ready";

        [Display(Name = "Camera Active")]
        public bool IsCameraActive { get; set; } = false;

        [Display(Name = "Printer Active")]
        public bool IsPrinterActive { get; set; } = true;

        [Display(Name = "Offline Mode")]
        public bool IsOfflineMode { get; set; } = false;

        [Display(Name = "Last Sync")]
        public DateTime LastSync { get; set; } = DateTime.Now;

        public List<VehicleExit> RecentExits { get; set; } = new();
    }

    public class VehicleEntry
    {
        public string LicensePlate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string GateId { get; set; } = string.Empty;
        public string OperatorId { get; set; } = string.Empty;
    }

    public class VehicleExit
    {
        public string LicensePlate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public decimal Fee { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string GateId { get; set; } = string.Empty;
        public string OperatorId { get; set; } = string.Empty;
    }
}