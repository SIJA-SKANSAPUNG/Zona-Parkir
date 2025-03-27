using System;

namespace Parking_Zone.Models
{
    public class FeeConfiguration
    {
        public Guid Id { get; set; }
        public string VehicleType { get; set; }
        public decimal BaseFee { get; set; }
        public Guid ParkingZoneId { get; set; }
        public virtual ParkingZone ParkingZone { get; set; }
    }
}