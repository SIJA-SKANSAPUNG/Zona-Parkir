using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.ViewModels
{
    public class ParkingRateViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Base Rate (First Hour)")]
        [Range(0, 1000000)]
        public decimal BaseRate { get; set; }

        [Required]
        [Display(Name = "Additional Rate (Per Hour)")]
        [Range(0, 1000000)]
        public decimal AdditionalRate { get; set; }

        [Required]
        [Display(Name = "Maximum Daily Rate")]
        [Range(0, 1000000)]
        public decimal MaxDailyRate { get; set; }

        [Display(Name = "Grace Period (Minutes)")]
        [Range(0, 60)]
        public int GracePeriodMinutes { get; set; } = 15;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; } = string.Empty;
    }
} 