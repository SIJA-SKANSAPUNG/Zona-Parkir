using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class EntryRequestModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string VehicleNumber { get; set; }

        [Required]
        public string VehicleType { get; set; }

        [Required]
        public int EntryGateId { get; set; }
        public virtual EntryGate EntryGate { get; set; }

        public DateTime RequestTime { get; set; }
        public string Status { get; set; }
        public string? DenialReason { get; set; }

        public string? ImagePath { get; set; }
        public string? QRCode { get; set; }
    }
}