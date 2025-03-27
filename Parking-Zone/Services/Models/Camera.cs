using System;

namespace Parking_Zone.Services.Models
{
    public class Camera
    {
        public string GateId { get; set; } = string.Empty;
        public bool IsOperational { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? LastSync { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Resolution { get; set; } = string.Empty;
        public string? LastError { get; set; }
    }
}
