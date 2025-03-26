using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class Printer
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

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime LastChecked { get; set; }

        public bool IsOnline { get; set; }

        public virtual ParkingGate Gate { get; set; }
        public int? GateId { get; set; }

        // Printer settings
        public int PaperWidth { get; set; } = 80; // mm
        public int CharactersPerLine { get; set; } = 48;
        public bool CutPaperAfterPrint { get; set; } = true;
        public bool OpenCashDrawerAfterPrint { get; set; } = false;
    }
} 