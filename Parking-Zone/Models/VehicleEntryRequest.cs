using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class VehicleEntryRequest
    {
        public Guid Id { get; set; }
        
        [Required]
        public string VehicleNumber { get; set; }
        
        [Required]
        public string VehicleType { get; set; }
        
        public string LicensePlate { get; set; }
        
        [Required]
        public string ParkingType { get; set; } = "Regular";
        
        public DateTime RequestTime { get; set; } = DateTime.Now;
        
        public string EntryGateId { get; set; }
        
        public string ImagePath { get; set; }
        
        public bool IsVerified { get; set; } = false;
        
        public string VerifiedBy { get; set; }
        
        public DateTime? VerificationTime { get; set; }
        
        public string Notes { get; set; }
        
        public bool IsProcessed { get; set; }
        
        public DateTime? ProcessedTime { get; set; }
        
        // Properties referenced in error messages
        public string PlateNumber { get; set; }
        public string GateId { get; set; }
        public string PhotoPath { get; set; }
        public bool PrintTicket { get; set; }
        public Guid? OperatorId { get; set; }

        // Navigation properties
        public virtual ParkingGate EntryGate { get; set; }
        public virtual Vehicle Vehicle { get; set; }
    }
}
