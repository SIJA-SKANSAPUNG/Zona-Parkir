namespace Parking_Zone.ViewModels.Reservation
{
    public class ReservationListItemVM
    {
        public ReservationListItemVM(Models.Reservation reservation)
        {
            StartTime = reservation.StartTime;
            Duration = reservation.Duration;
            VehicleNumber = reservation.VehicleNumber;
            SlotNumber = reservation.ParkingSlot.Number;
            ZoneAddress = reservation.ParkingSlot.ParkingZone.Address;
            ZoneName = reservation.ParkingSlot.ParkingZone.Name;
            IsActive = reservation.StartTime < DateTime.Now &&
                       reservation.StartTime.AddHours(reservation.Duration) > DateTime.Now;
        }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string VehicleNumber { get; set; }
        public int SlotNumber { get; set; }
        public string ZoneAddress { get; set; }
        public string ZoneName { get; set; }
        public bool IsActive { get; set; }
    }
}
