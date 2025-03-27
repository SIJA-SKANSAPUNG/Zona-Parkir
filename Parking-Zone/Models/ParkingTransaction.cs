using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class ParkingTransaction
    {
        [Key]
        public Guid Id { get; set; }

        public int? VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; }

        public Guid GateId { get; set; }
        [ForeignKey(nameof(GateId))]
        public ParkingGate Gate { get; set; }

        // Add EntryGateId property
        public Guid? EntryGateId { get; set; }
        [ForeignKey(nameof(EntryGateId))]
        public virtual EntryGate EntryGate { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        // Add LicensePlate and VehicleType properties
        [StringLength(50)]
        public string LicensePlate { get; set; }

        [StringLength(50)]
        public string VehicleType { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [ForeignKey("VehicleTypeId")]
        public virtual VehicleType VehicleTypeObject { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        public DateTime EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        [StringLength(500)]
        public string EntryPhotoPath { get; set; }

        [StringLength(500)]
        public string ExitPhotoPath { get; set; }

        [Required]
        public Guid OperatorId { get; set; }

        [ForeignKey("OperatorId")]
        public virtual AppUser Operator { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string Status { get; set; } = "Active";

        // Add ParkingFee and TicketNumber properties
        public decimal? ParkingFee { get; set; }
        
        [StringLength(50)]
        public string TicketNumber { get; set; }

        // Add TotalAmount and CancellationReason properties
        public decimal? TotalAmount { get; set; }

        [StringLength(500)]
        public string CancellationReason { get; set; }

        // Navigation properties
        public virtual ParkingZone ParkingZone { get; set; }
        public virtual ParkingSpace ParkingSpace { get; set; }

        // Add Foreign Keys
        public Guid? ParkingSpaceId { get; set; }
        public Guid ParkingZoneId { get; set; }
    }
}