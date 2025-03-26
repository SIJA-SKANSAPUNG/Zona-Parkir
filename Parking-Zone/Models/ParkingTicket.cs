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