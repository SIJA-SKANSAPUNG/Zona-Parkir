using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class Camera
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string IpAddress { get; set; } = string.Empty;

        public int Port { get; set; }

        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [StringLength(50)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Resolution { get; set; } = "1920x1080";

        public bool IsActive { get; set; } = true;

        public string Status { get; set; } = "Offline";

        [Required]
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;

        public string? LastError { get; set; }

        [ForeignKey("ParkingGate")]
        public Guid? ParkingGateId { get; set; }
        public virtual ParkingGate? ParkingGate { get; set; }

        public virtual CameraSettings? Settings { get; set; }

        public Guid? GateId { get; set; }
        public bool IsOperational { get; set; } = true;
        public string StatusCamera { get; set; } = "Active";
        public DateTime? LastMaintenanceDate { get; set; }
        public string SerialNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public string IPAddress { get; set; }
        public int PortCamera { get; set; } = 80;
        public bool IsNetworkCamera { get; set; } = true;
        public bool IsRecording { get; set; } = false;
        public int FrameRate { get; set; } = 30;
        public string StoragePath { get; set; }
    }
} 