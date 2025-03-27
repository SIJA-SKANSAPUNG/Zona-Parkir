namespace Parking_Zone.Hardware
{
    public class PrinterConfig
    {
        public string PaperSize { get; set; } = "80mm";
        public int DPI { get; set; } = 203;
        public bool AutoCut { get; set; } = true;
        public int CutterType { get; set; } = 0;
        public int CharactersPerLine { get; set; } = 42;
        public string FontName { get; set; } = "default";
        public int FontSize { get; set; } = 12;
        public bool BoldHeader { get; set; } = true;
        public string Alignment { get; set; } = "center";
        public int LineSpacing { get; set; } = 30;
        public bool PrintLogo { get; set; } = true;
        public string? LogoPath { get; set; }
    }
} 