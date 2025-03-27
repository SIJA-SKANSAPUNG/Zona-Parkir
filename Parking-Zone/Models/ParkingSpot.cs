using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkingSpot
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string SpotNumber { get; set; }
        
        [Required]
        public int ParkingZoneId { get; set; }
        public virtual ParkingZone ParkingZone { get; set; }
        
        public bool IsOccupied { get; set; }
        public bool IsReserved { get; set; }
        public bool IsDisabled { get; set; }
        
        public DateTime? LastOccupiedAt { get; set; }
        public DateTime? LastVacatedAt { get; set; }
        
        public string? CurrentVehicleId { get; set; }
        public virtual Vehicle? CurrentVehicle { get; set; }
    }
} 