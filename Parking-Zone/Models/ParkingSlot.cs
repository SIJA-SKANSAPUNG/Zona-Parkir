using Parking_Zone.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Parking_Zone.Models
{
    public class ParkingSlot
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailableForBooking { get; set; }
        public bool IsOccupied { get; set; }
        public SlotCategoryEnum Category { get; set; }
        [ForeignKey("ParkingZone")]
        public Guid ParkingZoneId { get; set; }
        public virtual ParkingZone ParkingZone { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; } = new List<ParkingTransaction>();

        [NotMapped]
        public bool HasAnyActiveReservation
        {
            get => Reservations?.Any(r => r.StartTime.AddHours(r.Duration) > DateTime.Now) ?? false;
        }
    }
}
