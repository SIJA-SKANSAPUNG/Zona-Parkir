using Parking_Zone.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class ParkingSlot
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailableForBooking { get; set; }
        public SlotCategoryEnum Category { get; set; }
        [ForeignKey("ParkingZone")]
        public Guid ParkingZoneId { get; set; }
        public virtual ParkingZone ParkingZone { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        [NotMapped]
        public bool HasAnyActiveReservation
        {
            get => Reservations.Any(r => r.StartTime.AddHours(r.Duration) > DateTime.Now);
        }
    }
}
