using System;

namespace Parking_Zone.Models
{
    public class PrinterConfig
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Port { get; set; } = null!;
        public string? ConnectionType { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastChecked { get; set; }
        public string? PaperSize { get; set; }
        public string? PrinterModel { get; set; }
        public string? DriverName { get; set; }
        public string? HeaderText { get; set; }
        public string? FooterText { get; set; }
        public bool PrintLogo { get; set; }
        public string? LogoPath { get; set; }
    }
}
