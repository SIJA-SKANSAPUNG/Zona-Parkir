using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Parking_Zone.Enums;

namespace Parking_Zone.Models
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ReservationNumber { get; set; } = null!;

        [Required]
        public Guid ParkingSpotId { get; set; }

        [ForeignKey(nameof(ParkingSpotId))]
        public virtual ParkingSpot ParkingSpot { get; set; } = null!;

        [Required]
        public Guid VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public virtual Vehicle Vehicle { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        [StringLength(100)]
        public string? CustomerName { get; set; }

        [StringLength(20)]
        public string? CustomerPhone { get; set; }

        [StringLength(100)]
        public string? CustomerEmail { get; set; }

        public decimal? TotalCost { get; set; }

        public DateTime? CancelledAt { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastUpdated { get; set; }

        // New properties for ReservationExtensions
        public DateTime? CompletedDateTime { get; set; }

        public TimeSpan Duration { get; set; }

        [ForeignKey(nameof(AppUserId))]
        public string? AppUserId { get; set; }
        public virtual ApplicationUser? AppUser { get; set; }
    }

    // Enum to support reservation status
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        InProgress,
        Completed,
        Cancelled
    }
}
