using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class Scanner
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string ConnectionString { get; set; }

        [StringLength(50)]
        public string Type { get; set; } // USB, Network, etc.

        [StringLength(50)]
        public string Model { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime LastChecked { get; set; }

        public bool IsOnline { get; set; }

        public virtual ParkingGate Gate { get; set; }
        public int? GateId { get; set; }

        // Scanner settings
        public string ScannerMode { get; set; } = "Continuous"; // Continuous, Manual
        public bool PlayBeepSound { get; set; } = true;
        public int ScanTimeout { get; set; } = 30; // seconds
        public string BarcodeFormat { get; set; } = "CODE128"; // CODE128, QR, etc.
    }
} 