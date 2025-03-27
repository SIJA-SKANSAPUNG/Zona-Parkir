using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class VehicleExit
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [Required]
        public Guid TransactionId { get; set; }

        [Required]
        public DateTime ExitTime { get; set; }

        [StringLength(500)]
        public string? ImagePath { get; set; }

        [StringLength(500)]
        public string? PlateImagePath { get; set; }

        public bool IsPlateVerified { get; set; }

        [StringLength(100)]
        public string? VerifiedBy { get; set; }

        public DateTime? VerificationTime { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Properties referenced in error messages
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        public TimeSpan Duration { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Pending";

        // New properties to resolve controller errors
        public Guid? OperatorId { get; set; }
        public Guid? GateId { get; set; }

        [StringLength(50)]
        public string? PlateNumber { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }

        [StringLength(500)]
        public string? ExitPhotoPath { get; set; }

        public Guid? ParkingSpaceId { get; set; }

        [StringLength(50)]
        public string? TicketNumber { get; set; }

        // Add missing properties
        [Required]
        public DateTime EntryTime { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [ForeignKey(nameof(TransactionId))]
        public virtual ParkingTransaction Transaction { get; set; } = null!;
    }
}
