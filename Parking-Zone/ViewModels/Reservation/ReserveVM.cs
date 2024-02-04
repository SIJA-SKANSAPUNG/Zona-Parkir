using Parking_Zone.Enums;
using System.CodeDom;

namespace Parking_Zone.ViewModels.Reservation
{
    public class ReserveVM
    {
        public ReserveVM()
        {
            
        }
        public ReserveVM(Models.ParkingSlot slot, string startTime, int duration)
        {
            SlotId = slot.Id;
            SlotNumber = slot.Number;
            SlotCategory = slot.Category;
            StartTime = startTime;
            Duration = duration;
            ZoneAddress = slot.ParkingZone.Address;
            ZoneName = slot.ParkingZone.Name;
        }
        public string ZoneName { get; set; }
        public string ZoneAddress { get; set; }
        public int SlotNumber { get; set; }
        public Guid SlotId { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string VehicleNumber { get; set; }
        public SlotCategoryEnum SlotCategory { get; set; }

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
