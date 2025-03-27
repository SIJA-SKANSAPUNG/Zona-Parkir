using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Win32;
using Parking_Zone.ViewModels.ParkingSlot;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels.Reservation
{
    public class FreeSlotsVM
    {
        public FreeSlotsVM()
        {
            
        }
        public FreeSlotsVM(IEnumerable<Models.ParkingZone> zones)
        {
            this.ParkingZones = new SelectList(zones, "Id", "Name");
        }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public Guid ParkingZoneId { get; set; }
        public SelectList ParkingZones { get; set; }
        public IEnumerable<ParkingSlotListItemVM> ParkingSlots { get; set; }
    }
}
