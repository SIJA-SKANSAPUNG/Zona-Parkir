using System;

namespace Parking_Zone.Services.Models
{
    public class CameraConfiguration
    {
        public string CameraId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 80;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string StreamUrl { get; set; } = string.Empty;
        public string SnapshotUrl { get; set; } = string.Empty;
        public int ImageQuality { get; set; } = 80;
        public int ImageResolutionWidth { get; set; } = 640;
        public int ImageResolutionHeight { get; set; } = 480;
    }
} 