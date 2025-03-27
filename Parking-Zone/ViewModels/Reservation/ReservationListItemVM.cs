using System.ComponentModel;

namespace Parking_Zone.ViewModels.Reservation
{
    public class ReservationListItemVM
    {
        public ReservationListItemVM()
        {
            
        }
        public ReservationListItemVM(Models.Reservation reservation)
        {
            Id = reservation.Id;
            StartTime = reservation.StartTime;
            Duration = reservation.Duration;
            VehicleNumber = reservation.VehicleNumber;
            SlotNumber = reservation.ParkingSlot.Number;
            ZoneAddress = reservation.ParkingSlot.ParkingZone.Address;
            ZoneName = reservation.ParkingSlot.ParkingZone.Name;
            IsOnGoing = reservation.IsOnGoing;
        }
        public Guid Id { get; set; }
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [DisplayName("Duration")]
        public int Duration { get; set; }
        [DisplayName("Vehicle Number")]
        public string VehicleNumber { get; set; }
        [DisplayName("Slot Number")]
        public int SlotNumber { get; set; }
        [DisplayName("Zone Address")]
        public string ZoneAddress { get; set; }
        [DisplayName("Zone Name")]
        public string ZoneName { get; set; }
        public bool IsOnGoing { get; set; }
    }
}
