using Parking_Zone.Enums;

namespace Parking_Zone.ViewModels.ParkingSlot
{
    public class ParkingSlotDetailsVM
    {
        public ParkingSlotDetailsVM()
        {

        }
        public ParkingSlotDetailsVM(Models.ParkingSlot slot)
        {
            this.Id = slot.Id;
            this.Number = slot.Number;
            this.IsAvailableForBooking = slot.IsAvailableForBooking;
            this.Category = slot.Category;
            this.ParkingZoneName = slot.ParkingZone.Name;
            this.ParkingZoneId = slot.ParkingZoneId;
            this.HasAnyActiveReservation = slot.HasAnyActiveReservation;
        }
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailableForBooking { get; set; }
        public SlotCategoryEnum Category { get; set; }
        public string ParkingZoneName { get; set; }
        public Guid ParkingZoneId { get; set; }
        public bool HasAnyActiveReservation { get; set; }
    }
}
