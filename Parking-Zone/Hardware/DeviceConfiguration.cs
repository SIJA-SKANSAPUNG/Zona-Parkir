using System.Collections.Generic;

namespace Parking_Zone.Hardware
{
    public class DeviceConfiguration
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public Dictionary<string, string> Settings { get; set; } = new();
    }
} 