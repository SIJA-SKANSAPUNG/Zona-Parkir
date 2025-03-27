using System;

namespace Parking_Zone.Models
{
    public class CameraSettings
    {
        public Guid Id { get; set; }
        public string ProfileName { get; set; } = null!;
        public string? LightingCondition { get; set; }
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Saturation { get; set; }
        public int Sharpness { get; set; }
        public bool AutoFocus { get; set; }
        public string? Resolution { get; set; }
        public int FrameRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastUpdated { get; set; }
        public string UpdatedBy { get; set; } = null!;
    }
}
