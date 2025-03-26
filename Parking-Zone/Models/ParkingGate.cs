using System;

namespace Parking_Zone.Models
{
    public class ParkingGate
    {
        public Guid Id { get; set; }
        public string Name { get; set; } // "Entry" atau "Exit"
        public string DeviceId { get; set; }
        public bool IsOpen { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastActivity { get; set; }
        public Guid ParkingZoneId { get; set; }
        
        // Navigation properties
        public virtual ParkingZone ParkingZone { get; set; }
    }
} 