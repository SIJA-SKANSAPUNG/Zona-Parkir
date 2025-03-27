using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [ForeignKey("VehicleTypeId")]
        public virtual VehicleType VehicleTypeNavigation { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public string ExitPhotoPath { get; set; }

        // Properties referenced in error messages
        public string VehicleNumber { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Additional properties
        public string PlateNumber { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public string PhotoEntry { get; set; }
        public string PhotoExit { get; set; }
        public bool IsInside { get; set; }
        public string TicketBarcode { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string DriverName { get; set; }
        public string PhoneNumber { get; set; }
        public string EntryPhotoPath { get; set; }
        public Guid? ParkingSpaceId { get; set; }
        public Guid? ShiftId { get; set; }
        public string BarcodeImagePath { get; set; }
        public bool IsParked { get; set; }

        // Navigation properties
        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; }
        public virtual ICollection<VehicleEntry> VehicleEntries { get; set; }
        public virtual ICollection<VehicleExit> VehicleExits { get; set; }

        public Vehicle()
        {
            ParkingTransactions = new HashSet<ParkingTransaction>();
            VehicleEntries = new HashSet<VehicleEntry>();
            VehicleExits = new HashSet<VehicleExit>();
        }
    }
}