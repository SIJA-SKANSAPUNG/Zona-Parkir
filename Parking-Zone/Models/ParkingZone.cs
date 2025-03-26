using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkingZone
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        public int Capacity { get; set; }

        public int AvailableSpaces { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastActivity { get; set; }

        // Navigation properties
        public virtual ICollection<ParkingGate> Gates { get; set; }

        public ParkingZone()
        {
            Gates = new HashSet<ParkingGate>();
        }
    }
}