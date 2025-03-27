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
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [Required]
        public Guid ParkingZoneId { get; set; }

        [ForeignKey("ParkingZoneId")]
        public virtual ParkingZone ParkingZone { get; set; }

        [Required]
        public Guid SlotId { get; set; }

        [ForeignKey("SlotId")]
        public virtual ParkingSlot ParkingSlot { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        [StringLength(50)]
        public string VehicleNumber { get; set; }

        [Required]
        public int ParkingSpotId { get; set; }

        [ForeignKey("ParkingSpotId")]
        public virtual ParkingSpot ParkingSpot { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? VehiclePlateNumber { get; set; }

        [ForeignKey("VehiclePlateNumber")]
        public virtual Vehicle Vehicle { get; set; }

        public Enums.ReservationStatus Status { get; set; } = Enums.ReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        public decimal? TotalFee { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? PaymentReference { get; set; }
        public string? Notes { get; set; }

        [NotMapped]
        public bool IsOnGoing
        {
            get
            {
                var now = DateTime.Now;
                return now >= StartTime && now <= StartTime.AddHours(Duration);
            }
        }
    }
}
