using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkingZone
    {
        public Guid Id { get; set; }

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

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastActivity { get; set; }

        public DateTime DateOfEstablishment { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ParkingGate> Gates { get; set; }
        public virtual ICollection<ParkingSlot> ParkingSlots { get; set; }
        public virtual ICollection<Rate> Rates { get; set; }
        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; }
        public virtual ICollection<VehicleType> VehicleTypes { get; set; } = new HashSet<VehicleType>();

        public ParkingZone()
        {
            Gates = new HashSet<ParkingGate>();
            ParkingSlots = new HashSet<ParkingSlot>();
            Rates = new HashSet<Rate>();
            ParkingTransactions = new HashSet<ParkingTransaction>();
            VehicleTypes = new HashSet<VehicleType>();
        }
    }
}