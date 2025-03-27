using System;

namespace Parking_Zone.ViewModels
{
    public enum VehicleType
    {
        // Add enum values as needed
    }

    public class VehicleEntryModel : BaseViewModel
    {
        public string LicensePlate { get; set; } = null!;
        public VehicleType VehicleType { get; set; }
        public DateTime EntryTime { get; set; }
        public Guid OperatorId { get; set; }
        public byte[]? PhotoEntry { get; set; }
        public string TicketBarcode { get; set; } = string.Empty;
        public Guid? ParkingSpaceId { get; set; }
        public string? PhotoPath { get; set; }
        public string? Notes { get; set; }
        public Guid? GateId { get; set; }
    }
}
