﻿using Parking_Zone.Models;
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
            SlotNumber = reservation.ParkingSlot.Number;
            ZoneAddress = reservation.ParkingSlot.ParkingZone.Address;
            EndDateTime = reservation.StartTime.AddHours(reservation.Duration).ToString();
        }
        public Guid ReservationId { get; set; }
        public string? EndDateTime { get; set; }
        public int ExtraHours { get; set; }
        public int? SlotNumber { get; set; }
        public string? ZoneAddress { get; set; }
    }
}
