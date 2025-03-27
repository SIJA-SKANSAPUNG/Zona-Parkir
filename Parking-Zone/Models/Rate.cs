using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class Rate
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string VehicleType { get; set; }
        
        [Required]
        public decimal HourlyRate { get; set; }
        
        [Required]
        public decimal DailyRate { get; set; }
        
        public decimal? WeeklyRate { get; set; }
        
        public decimal? MonthlyRate { get; set; }
        
        public decimal FlatRate { get; set; }
        
        [Required]
        public decimal MinimumFee { get; set; }
        
        public int FreeMinutes { get; set; } = 15;
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public string CreatedBy { get; set; }
        
        public string UpdatedBy { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }

        // Zone specific rates
        public int? ZoneId { get; set; }
        public virtual ParkingZone Zone { get; set; }
    }
}
