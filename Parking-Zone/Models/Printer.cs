using System;
using System.ComponentModel.DataAnnotations;
using Parking_Zone.Hardware;

namespace Parking_Zone.Models
{
    public class Printer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ConnectionString { get; set; } = string.Empty;

        [StringLength(50)]
        public string Type { get; set; } = string.Empty; // USB, Network, etc.

        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOperational { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
        public DateTime LastChecked { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }

        public bool IsOnline { get; set; }
        public bool NeedsMaintenance { get; set; }

        public string Status { get; set; } = "Ready";
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int PaperLevel { get; set; } = 100;

        public string GateId { get; set; } = string.Empty;
        public virtual ParkingGate? Gate { get; set; }

        public PrinterConfig? Config { get; set; }
    }
} 