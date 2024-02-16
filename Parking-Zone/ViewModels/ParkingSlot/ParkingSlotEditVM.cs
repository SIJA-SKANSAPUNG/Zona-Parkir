using Parking_Zone.Enums;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels.ParkingSlot
{
    public class ParkingSlotEditVM
    {
        public ParkingSlotEditVM()
        {

        }
        public ParkingSlotEditVM(Models.ParkingSlot slot)
        {
            this.Id = slot.Id;
            this.Number = slot.Number;
            this.ParkingZoneId = slot.ParkingZoneId;
            this.IsAvailableForBooking = slot.IsAvailableForBooking;
            this.Category = slot.Category;
            this.ParkingZoneName = slot.ParkingZone.Name;
            this.HasAnyActiveReservation = slot.Reservations.Any(r => r.IsActive == true);
        }
        public Guid Id { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public bool IsAvailableForBooking { get; set; }
        [Required]
        public SlotCategoryEnum Category { get; set; }
        [Required]
        public Guid ParkingZoneId { get; set; }
        public string? ParkingZoneName { get; set; }
        public bool HasAnyActiveReservation { get; set; } = false;

        public Models.ParkingSlot MapToModel(Models.ParkingSlot slot)
        {
            slot.Number = this.Number;
            slot.ParkingZoneId = this.ParkingZoneId;
            slot.Category = this.Category;
            slot.IsAvailableForBooking = this.IsAvailableForBooking;
            return slot;
        }
    }
}
