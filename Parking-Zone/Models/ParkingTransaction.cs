using System;

namespace Parking_Zone.Models
{
    public class ParkingTransaction
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public string ReceiptNumber { get; set; }
        public Guid? OperatorId { get; set; }
        public string OperatorName { get; set; }
        public Guid ParkingZoneId { get; set; }
        
        // Navigation properties
        public virtual ParkingZone ParkingZone { get; set; }
    }
} 