using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parking_Zone.Enums;

namespace Parking_Zone.Models
{
    public class VehicleEntry
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [Required]
        public DateTime EntryTime { get; set; }

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

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string LicensePlate { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public Guid GateId { get; set; }

        [ForeignKey("GateId")]
        public virtual ParkingGate Gate { get; set; } = null!;

        public Guid? ParkingSpaceId { get; set; }

        [ForeignKey("ParkingSpaceId")]
        public virtual ParkingSlot? ParkingSlot { get; set; }

        // Matching ViewModel properties
        public string? EntryOperator { get; set; }
        public string? PhotoEntry { get; set; }
        public string? TicketBarcode { get; set; }
    }
}
