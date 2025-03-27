using System;

namespace Parking_Zone.Hardware
{
    public abstract class HardwareConfiguration
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public int BaudRate { get; set; } = 9600;
        public bool IsActive { get; set; } = true;
    }

    public class CameraConfiguration : HardwareConfiguration
    {
        public string Resolution { get; set; } = "1920x1080";
        public string Format { get; set; } = "JPEG";
        public string IpAddress { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SavePath { get; set; } = string.Empty;
    }

    public class PrinterConfiguration : HardwareConfiguration
    {
        public string PaperSize { get; set; } = "80mm";
        public bool IsThermal { get; set; } = true;
        public int CharactersPerLine { get; set; } = 42;
        public string Encoding { get; set; } = "ASCII";
        public bool CutPaperAfterPrint { get; set; } = true;
    }

    public class ScannerConfiguration : HardwareConfiguration
    {
        public string ScanMode { get; set; } = "Barcode";
        public string CodeType { get; set; } = "QR";
        public bool AutoScan { get; set; } = true;
        public int ScanInterval { get; set; } = 1000;
        public string TriggerCharacter { get; set; } = "\r";
    }

    public class GateConfiguration : HardwareConfiguration
    {
        public string Direction { get; set; } = "Entry";
        public int OpenTime { get; set; } = 5000;
        public bool HasSensor { get; set; } = true;
        public string OpenSignal { get; set; } = "1";
        public string CloseSignal { get; set; } = "0";
    }
} 