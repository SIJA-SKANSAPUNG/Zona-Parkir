using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class ParkingGate
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Location { get; set; }

        public string GateType { get; set; }  // "Entry", "Exit", "Both"

        [Required]
        public string Status { get; set; } = "Online";  // Online, Offline, Maintenance, Disabled

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsOpen { get; set; }
        public bool IsOperational { get; set; } = true;

        [Required]
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastActivity { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;

        // Hardware relationships
        public Guid? CameraId { get; set; }
        public virtual Camera Camera { get; set; }

        public Guid? PrinterId { get; set; }
        public virtual Printer Printer { get; set; }

        public Guid? ScannerId { get; set; }
        public virtual Scanner Scanner { get; set; }

        // Navigation properties
        [ForeignKey("ParkingZone")]
        public Guid ParkingZoneId { get; set; }
        public virtual ParkingZone ParkingZone { get; set; }
        public virtual ICollection<GateOperation> Operations { get; set; }

        public ParkingGate()
        {
            Operations = new HashSet<GateOperation>();
        }
    }
}