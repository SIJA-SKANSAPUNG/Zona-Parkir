using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class PrinterConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PortName { get; set; } = string.Empty;

        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "One";
        public string Handshake { get; set; } = "None";

        public bool IsEnabled { get; set; } = true;
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? SerialNumber { get; set; }

        public int PaperWidth { get; set; } = 80;
        public int CharactersPerLine { get; set; } = 48;
        public bool AutoCut { get; set; } = true;
        public int CutLength { get; set; } = 0;

        public string? HeaderText { get; set; }
        public string? FooterText { get; set; }
        public bool PrintLogo { get; set; }
        public string? LogoPath { get; set; }
    }
} 