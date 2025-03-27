using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels
{
    public class EntryRequest
    {
        [Required]
        public string VehicleNumber { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }

        [Required]
        public Guid GateId { get; set; }

        public string ImagePath { get; set; }

        [Required]
        public string LicensePlate { get; set; }
    }
}
