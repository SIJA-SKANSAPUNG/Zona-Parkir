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
        [StringLength(50)]
        public string LicensePlate { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string PlateNumber { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = null!;

        [Required]
        public Guid VehicleTypeId { get; set; }

        [ForeignKey(nameof(VehicleTypeId))]
        public virtual VehicleType VehicleTypeNavigation { get; set; } = null!;

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        [StringLength(100)]
        public string? EntryOperator { get; set; }

        [StringLength(100)]
        public string? ExitOperator { get; set; }

        public byte[]? PhotoEntry { get; set; }
        public byte[]? PhotoExit { get; set; }

        [StringLength(50)]
        public string? TicketBarcode { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsInside { get; set; } = false;
        public bool IsParked { get; set; } = false;

        [StringLength(50)]
        public string? Type { get; set; }

        // New property to resolve ParkingTransactionController error
        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdated { get; set; }

        // New properties to resolve GateController errors
        [StringLength(500)]
        public string? ExitPhotoPath { get; set; }

        // Navigation properties
        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; } = new HashSet<ParkingTransaction>();
        public virtual ICollection<VehicleEntry> VehicleEntries { get; set; } = new HashSet<VehicleEntry>();
        public virtual ICollection<VehicleExit> VehicleExits { get; set; } = new HashSet<VehicleExit>();
    }
}