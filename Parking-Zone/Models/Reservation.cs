﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string VehicleNumber { get; set; }
        [ForeignKey("ParkingSlot")]
        public Guid SlotId { get; set; }
        public virtual ParkingSlot ParkingSlot { get; set; }
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }

        [NotMapped]
        public bool IsActive
        {
            get => StartTime <= DateTime.Now && DateTime.Now < StartTime.AddHours(Duration);
        }
    }
}
