using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels
{
    public class EntryRequestViewModel : BaseViewModel
    {
        [Required]
        public string VehicleNumber { get; set; } = null!;

        [Required]
        public string VehicleType { get; set; } = null!;

        [Required]
        public Guid GateId { get; set; }

        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
        public string? DenialReason { get; set; }

        public string? PhotoPath { get; set; }
        public string? ImagePath { get; set; }
        public string? QRCode { get; set; }

        public string PlateNumber { get; set; } = null!;
        public Guid OperatorId { get; set; }
        public string? TicketBarcode { get; set; }
        public string? EntryOperator { get; set; }
        public string? PhotoEntry { get; set; }

        // Add missing properties
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;
        public byte[]? ImageBytes { get; set; }
    }
}
