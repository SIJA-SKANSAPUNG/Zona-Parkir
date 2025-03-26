using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public ShiftStatus Status { get; set; } = ShiftStatus.Scheduled;

        // Navigation properties
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }

        public virtual ParkingGate Gate { get; set; }
        public int GateId { get; set; }
    }

    public enum ShiftStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Missed,
        Cancelled
    }
} 