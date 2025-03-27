using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkingTicket
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TicketNumber { get; set; }

        [Required]
        public DateTime IssuedAt { get; set; }

        public DateTime? ValidUntil { get; set; }

        public bool IsVoided { get; set; }

        [StringLength(500)]
        public string VoidReason { get; set; }

        public DateTime? VoidedAt { get; set; }

        public bool IsPrinted { get; set; }

        public int PrintCount { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public string VehicleNumber { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? IssueTime { get; set; }
        public string Status { get; set; }
        public string BarcodeData { get; set; }
        public string BarcodeImagePath { get; set; }
        public string ParkingSpaceNumber { get; set; }
        public string VehicleType { get; set; }
        public Guid? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public Guid? OperatorId { get; set; }
        public Guid? ShiftId { get; set; }

        // Navigation properties
        public virtual ParkingTransaction Transaction { get; set; }
        public int TransactionId { get; set; }

        public virtual Operator IssuedBy { get; set; }
        public int IssuedById { get; set; }

        public virtual Operator VoidedBy { get; set; }
        public int? VoidedById { get; set; }

        public virtual ParkingGate Gate { get; set; }
        public int GateId { get; set; }
    }
}