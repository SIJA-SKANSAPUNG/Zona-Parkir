using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public int ParkingZoneId { get; set; }

        [ForeignKey("ParkingZoneId")]
        public virtual ParkingZone ParkingZone { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? VehiclePlateNumber { get; set; }

        [ForeignKey("VehiclePlateNumber")]
        public virtual Vehicle Vehicle { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        public decimal? TotalFee { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentReference { get; set; }

        public string? Notes { get; set; }
    }

    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        InUse,
        Completed,
        Cancelled,
        Expired
    }
}
