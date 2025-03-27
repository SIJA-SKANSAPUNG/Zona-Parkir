using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class ParkingTransaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Required]
        [StringLength(20)]
        public string VehicleLicensePlate { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [ForeignKey("VehicleTypeId")]
        public virtual VehicleType VehicleType { get; set; }

        [Required]
        public Guid EntryGateId { get; set; }

        [ForeignKey("EntryGateId")]
        public virtual EntryGate EntryGate { get; set; }

        [Required]
        public Guid ExitGateId { get; set; }

        [ForeignKey("ExitGateId")]
        public virtual ExitGate ExitGate { get; set; }

        [Required]
        public DateTime EntryTime { get; set; }

        public DateTime? ExitTime { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string TicketNumber { get; set; }

        public decimal? ParkingFee { get; set; }

        public string EntryPhotoPath { get; set; }

        public string ExitPhotoPath { get; set; }

        [Required]
        public Guid OperatorId { get; set; }

        [ForeignKey("OperatorId")]
        public virtual AppUser Operator { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Status { get; set; } = "Active";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPaid { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation properties
        public virtual ParkingZone ParkingZone { get; set; }
        public virtual ParkingSpace ParkingSpace { get; set; }

        // Add Foreign Keys
        public Guid? ParkingSpaceId { get; set; }
        public Guid ParkingZoneId { get; set; }
    }
}