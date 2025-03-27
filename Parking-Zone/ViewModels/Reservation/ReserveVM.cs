using Parking_Zone.Enums;
using System.CodeDom;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels.Reservation
{
    public class ReserveVM
    {
        public ReserveVM()
        {
            
        }
        public ReserveVM(Models.ParkingSlot slot, string startTime, int duration)
        {
            StartTime = startTime;
            Duration = duration;
            SlotId = slot.Id;
            ZoneAddress = slot.ParkingZone.Address;
            ZoneName = slot.ParkingZone.Name;
            SlotNumber = slot.Number;
        }
        [Required]
        public Guid SlotId { get; set; }
        public int SlotNumber { get; set; }
        public string ZoneName { get; set; }
        public string ZoneAddress { get; set; }
        [Required]
        public string StartTime { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public string VehicleNumber { get; set; }

        public Models.Reservation MapToModel()
        {
            return new Models.Reservation()
            {
                Duration = this.Duration,
                SlotId = this.SlotId,
                StartTime = DateTime.Parse(this.StartTime),
                VehicleNumber = this.VehicleNumber,
            };
        }
    }
}
