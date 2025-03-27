using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class ParkingTicket
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TicketNumber { get; set; } = null!;

        [Required]
        public DateTime IssuedAt { get; set; }

        public DateTime? ValidUntil { get; set; }

        public bool IsVoided { get; set; }

        [StringLength(500)]
        public string? VoidReason { get; set; }

        public DateTime? VoidedAt { get; set; }

        public bool IsPrinted { get; set; }

        public int PrintCount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? VehicleNumber { get; set; }

        public DateTime? EntryTime { get; set; }
        public DateTime? IssueTime { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(500)]
        public string? BarcodeData { get; set; }

        [StringLength(500)]
        public string? BarcodeImagePath { get; set; }

        [StringLength(50)]
        public string? ParkingSpaceNumber { get; set; }

        [StringLength(50)]
        public string? VehicleType { get; set; }

        public Guid? VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle? Vehicle { get; set; }

        public Guid? OperatorId { get; set; }
        public Guid? ShiftId { get; set; }

        // Navigation properties
        public Guid? TransactionId { get; set; }

        [ForeignKey(nameof(TransactionId))]
        public virtual ParkingTransaction? Transaction { get; set; }

        public Guid? IssuedById { get; set; }

        [ForeignKey(nameof(IssuedById))]
        public virtual Operator? IssuedBy { get; set; }

        public Guid? VoidedById { get; set; }

        [ForeignKey(nameof(VoidedById))]
        public virtual Operator? VoidedBy { get; set; }

        public Guid? GateId { get; set; }

        [ForeignKey(nameof(GateId))]
        public virtual ParkingGate? Gate { get; set; }

        // Additional properties
        public bool IsUsed { get; set; }

        public DateTime? ScanTime { get; set; }

        public string VehicleLicensePlate { get; set; } = string.Empty;
    }
}