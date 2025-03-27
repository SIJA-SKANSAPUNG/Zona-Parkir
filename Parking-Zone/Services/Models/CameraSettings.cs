using System;

namespace Parking_Zone.Services.Models
{
    public class CameraSettings
    {
        public string GateId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 80;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Resolution { get; set; } = "1920x1080";
        public string Format { get; set; } = "JPEG";
    }
}
