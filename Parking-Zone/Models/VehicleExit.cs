using System;

namespace Parking_Zone.Models
{
    public class VehicleExit
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime ExitTime { get; set; }
        public string? ImagePath { get; set; }
        public string? PlateImagePath { get; set; }
        public bool IsPlateVerified { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerificationTime { get; set; }
        public string? Notes { get; set; }

        // Properties referenced in error messages
        public decimal Cost { get; set; }
        public TimeSpan Duration { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string PaymentStatus { get; set; } = "Pending";

        public virtual Vehicle Vehicle { get; set; } = null!;
        public virtual ParkingTransaction Transaction { get; set; } = null!;
    }
}
