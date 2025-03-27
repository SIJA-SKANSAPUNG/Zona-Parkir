using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parking_Zone.Enums;

namespace Parking_Zone.Models
{
    public class ParkingTransaction
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VehicleId { get; set; }
        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; }

        public Guid GateId { get; set; }
        [ForeignKey(nameof(GateId))]
        public ParkingGate Gate { get; set; }

        public Guid? EntryGateId { get; set; }
        [ForeignKey(nameof(EntryGateId))]
        public virtual EntryGate EntryGate { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [StringLength(50)]
        public string LicensePlate { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = null!;

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

        public TimeSpan? Duration { get; set; }

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

        public decimal? ParkingFee { get; set; }
        
        [StringLength(50)]
        public string TicketNumber { get; set; }

        public decimal? TotalAmount { get; set; }

        [StringLength(500)]
        public string CancellationReason { get; set; }

        public virtual ParkingZone ParkingZone { get; set; }
        public virtual ParkingSpace ParkingSpace { get; set; }

        [Required]
        public Guid ParkingSpaceId { get; set; }

        public Guid ParkingZoneId { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionNumber { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Fee { get; set; }

        [NotMapped]
        public string? EntryTimeFormatted { get; private set; }

        [NotMapped]
        public string? ExitTimeFormatted { get; private set; }

        [NotMapped]
        public bool IsExit => ExitTime.HasValue;

        public string? EntryOperator { get; set; }

        public string? ExitOperator { get; set; }

        public ParkingTransactionStatus TransactionStatus { get; set; }

        public DateTime? LastUpdated { get; set; }

        [StringLength(100)]
        public string? Notes { get; set; }

        // New properties for payment and transaction details
        public string? PaymentStatus { get; set; }
        public DateTime? PaymentTime { get; set; }

        // Entry and Exit Points
        [StringLength(50)]
        public string? EntryPoint { get; set; }

        [StringLength(50)]
        public string? ExitPoint { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleLicensePlate { get; set; } = null!;

        [Required]
        public Guid EntryOperatorId { get; set; }

        public Guid? ExitOperatorId { get; set; }

        public void SetFormattedTimes()
        {
            EntryTimeFormatted = EntryTime.ToString("yyyy-MM-dd HH:mm:ss");
            ExitTimeFormatted = ExitTime?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public enum ParkingTransactionStatus
        {
            Pending,
            Completed,
            Cancelled,
            Refunded
        }
    }
}