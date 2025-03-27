using System;

namespace Parking_Zone.ViewModels
{
    public class ExitReceiptViewModel
    {
        public string LicensePlate { get; set; } = null!;
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal TotalFee { get; set; }
        public decimal ParkingFee { get; set; }
        public string? TicketNumber { get; set; }
        public VehicleType VehicleType { get; set; }
        public Guid? ParkingSpaceId { get; set; }
        public string? Notes { get; set; }
    }
}
