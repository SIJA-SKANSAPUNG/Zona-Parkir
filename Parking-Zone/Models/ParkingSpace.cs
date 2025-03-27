using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkingSpace
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SpaceNumber { get; set; } = string.Empty;

        [Required]
        public int ParkingZoneId { get; set; }
        public virtual ParkingZone? ParkingZone { get; set; }

        public bool IsOccupied { get; set; }
        public bool IsReserved { get; set; }
        public bool IsDisabled { get; set; }
        
        // Properties referenced in error messages
        public bool IsActive { get; set; } = true;
        public string Type { get; set; } = "Standard";
        public string SpaceType { get; set; } = "Standard";

        public DateTime? LastOccupiedTime { get; set; }
        public DateTime? LastVacatedTime { get; set; }

        public string? CurrentVehicleId { get; set; }
        public virtual Vehicle? CurrentVehicle { get; set; }

        public string? ReservationId { get; set; }
        public virtual Reservation? Reservation { get; set; }

        public string Status { get; set; } = "Available";
        public string? Notes { get; set; }

        // New property to resolve ExitGateController error
        public string? Name { get; set; }
    }
}