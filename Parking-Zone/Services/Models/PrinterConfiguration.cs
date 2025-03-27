using System;

namespace Parking_Zone.Services.Models
{
    public class PrinterConfiguration
    {
        public string PrinterId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PortName { get; set; } = string.Empty;
        public int BaudRate { get; set; } = 9600;
        public int DataBits { get; set; } = 8;
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "One";
        public string Handshake { get; set; } = "None";
        public string Model { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int PaperWidth { get; set; } = 80;
        public int CharactersPerLine { get; set; } = 48;
        public bool AutoCut { get; set; } = true;
        public int CutLength { get; set; } = 0;
        public string HeaderText { get; set; } = string.Empty;
        public string FooterText { get; set; } = string.Empty;
        public bool PrintLogo { get; set; } = false;
        public string LogoPath { get; set; } = string.Empty;
    }
} 