using System;

namespace Parking_Zone.Models
{
    public class SiteSettings
    {
        public Guid Id { get; set; }
        public string SiteName { get; set; } = null!;
        public string? ThemeColor { get; set; }
        public bool ShowLogo { get; set; }
        public string? LogoPath { get; set; }
        public string? LogoUrl { get; set; }
        public string? FaviconPath { get; set; }
        public string? WelcomeMessage { get; set; }
        public string? FooterText { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public bool EnableNotifications { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; } = null!;
        
        // New properties
        public string? Theme { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? TimeFormat { get; set; }
        public string? DateFormat { get; set; }
    }
}
