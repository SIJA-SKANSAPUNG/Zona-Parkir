using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkingGate
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        public GateType Type { get; set; }

        public GateStatus Status { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastActivity { get; set; }

        // Hardware relationships
        public virtual ICollection<Camera> Cameras { get; set; }
        public virtual ICollection<Printer> Printers { get; set; }
        public virtual ICollection<Scanner> Scanners { get; set; }

        // Navigation properties
        public virtual ParkingZone ParkingZone { get; set; }
        public int ParkingZoneId { get; set; }

        public ParkingGate()
        {
            Cameras = new HashSet<Camera>();
            Printers = new HashSet<Printer>();
            Scanners = new HashSet<Scanner>();
        }
    }

    public enum GateType
    {
        Entry,
        Exit,
        Both
    }

    public enum GateStatus
    {
        Online,
        Offline,
        Maintenance,
        Disabled
    }
} 