using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class GateOperation
    {
        public Guid Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        public string OperationType { get; set; }  // OPEN, CLOSE, ERROR

        public string Status { get; set; }  // SUCCESS, FAILED

        public string Details { get; set; }

        public Guid? ParkingTransactionId { get; set; }

        public Guid? EntryGateId { get; set; }

        public Guid? ExitGateId { get; set; }

        public Guid? OperatorId { get; set; }

        [ForeignKey("EntryGateId")]
        public virtual EntryGate EntryGate { get; set; }

        [ForeignKey("ExitGateId")]
        public virtual ExitGate ExitGate { get; set; }

        [ForeignKey("OperatorId")]
        public virtual Operator Operator { get; set; }

        public string OperatorName { get; set; }

        public string ErrorMessage { get; set; }

        public string TriggerSource { get; set; }  // MANUAL, AUTOMATIC, SCHEDULED
        public string Notes { get; set; }

        [ForeignKey("ParkingTransactionId")]
        public virtual ParkingTransaction ParkingTransaction { get; set; }
    }
}
