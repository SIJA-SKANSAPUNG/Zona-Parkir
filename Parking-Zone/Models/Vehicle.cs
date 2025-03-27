using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class Vehicle
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string PlateNumber { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string VehicleTypeId { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; } = null!;
        
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? EntryTime { get; set; }
        
        public DateTime? ExitTime { get; set; }
        
        public bool IsInside { get; set; }
        
        public byte[]? PhotoEntry { get; set; }
        
        public byte[]? PhotoExit { get; set; }
        
        public string? TicketBarcode { get; set; }
        
        public DateTime? LastUpdated { get; set; }
        
        // Navigation property
        [ForeignKey("VehicleTypeId")]
        public virtual VehicleType? VehicleTypeNavigation { get; set; }
    }
}