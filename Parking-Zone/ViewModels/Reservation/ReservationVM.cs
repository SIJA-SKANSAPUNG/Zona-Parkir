using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels.Reservation
{
    public class ReservationVM
    {
        [Required]
        public int Duration { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public Guid ParkingZoneId { get; set; }
        public IEnumerable<Models.ParkingSlot> ParkingSlots { get; set; }
        public SelectList ParkingZones { get; set; }
    }
}
