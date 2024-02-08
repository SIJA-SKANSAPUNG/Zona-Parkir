namespace Parking_Zone.ViewModels.Reservation
{
    public class ReservationListItemVM
    {
        public ReservationListItemVM(Models.Reservation reservation)
        {
            this.StartTime = reservation.StartTime.ToString();
            this.Duration = reservation.Duration;
            this.VehicleNumber = reservation.VehicleNumber;
            this.SlotNumber = reservation.ParkingSlot.Number;
            this.ZoneAddress = reservation.ParkingSlot.ParkingZone.Address;
            this.ZoneName = reservation.ParkingSlot.ParkingZone.Name;
            this.IsActive = reservation.StartTime.AddHours(reservation.Duration) > DateTime.Now;

        }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public string VehicleNumber { get; set; }
        public int SlotNumber { get; set; }
        public string ZoneAddress { get; set; }
        public string ZoneName { get; set; }
        public bool IsActive { get; set; }
    }
}
