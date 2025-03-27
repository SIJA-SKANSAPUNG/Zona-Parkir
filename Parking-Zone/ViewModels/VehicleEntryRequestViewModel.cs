using System;
using System.ComponentModel.DataAnnotations;
using Parking_Zone.Enums;

namespace Parking_Zone.ViewModels
{
    public class VehicleEntryRequestViewModel
    {
        [Required]
        public string PlateNumber { get; set; } = null!;

        [Required]
        public VehicleType VehicleType { get; set; }

        public string? EntryOperator { get; set; }
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;
        public byte[]? PhotoEntry { get; set; }
        public string? TicketBarcode { get; set; }

        // Added missing properties
        public Guid? ParkingSpaceId { get; set; }
        public string? PhotoPath { get; set; }
        public string? Notes { get; set; }
        public Guid? GateId { get; set; }
        public bool PrintTicket { get; set; } = false;
        public Guid VehicleId { get; set; } = Guid.NewGuid();
    }
}
