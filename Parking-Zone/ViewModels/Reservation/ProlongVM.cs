using Parking_Zone.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels.Reservation
{
    public class ProlongVM
    {
        public ProlongVM()
        {
            
        }
        public ProlongVM(Models.Reservation reservation)
        {
            ReservationId = reservation.Id;
            StartTime = reservation.StartTime.ToString();
            OldDuration = reservation.Duration;
            VehicleNumber = reservation.VehicleNumber;
            SlotNumber = reservation.ParkingSlot.Number;
            ZoneAddress = reservation.ParkingSlot.ParkingZone.Address;
            EndDateTime = reservation.StartTime.AddHours(reservation.Duration).ToString();
            IsActive = true;
        }
        public Guid ReservationId { get; set; }
        public string StartTime { get; set; }
        public string EndDateTime { get; set; }
        public int OldDuration { get; set; }
        [Required]
        public int NewDuration { get; set; }
        public string VehicleNumber { get; set; }
        public int SlotNumber { get; set; }
        public string ZoneAddress { get; set; }
        public bool IsActive { get; set; }
    }
}
