using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class VehicleEntryModel
    {
        [Required]
        public string VehicleNumber { get; set; } = string.Empty;

        [Required]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        public int EntryGateId { get; set; }

        public string? DriverName { get; set; }
        public string? DriverContact { get; set; }
        public string? Purpose { get; set; }

        public bool IsReservationHolder { get; set; }
        public string? ReservationCode { get; set; }

        public string? ImagePath { get; set; }
        public DateTime EntryTime { get; set; } = DateTime.UtcNow;
    }
} 