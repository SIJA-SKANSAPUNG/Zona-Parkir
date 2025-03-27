using System;

namespace Parking_Zone.ViewModels
{
    public class VehicleEntryViewModel
    {
        public string LicensePlate { get; set; } = null!;
        public VehicleType VehicleType { get; set; }
        public DateTime EntryTime { get; set; }
        public string? EntryOperator { get; set; }
        public byte[]? PhotoEntry { get; set; }
        public string? TicketBarcode { get; set; }
        public Guid? ParkingSpaceId { get; set; }
        public Guid? OperatorId { get; set; }
        public string? Notes { get; set; }
    }
}
