using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class CameraConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DeviceId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [Required]
        public int BaudRate { get; set; }

        [Required]
        [StringLength(50)]
        public string Resolution { get; set; }

        [Required]
        [StringLength(50)]
        public string Format { get; set; }

        [Required]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; }

        public string? Username { get; set; }
        public string? Password { get; set; }

        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? SerialNumber { get; set; }

        public string? StreamUrl { get; set; }
        public string? SnapshotUrl { get; set; }

        public int? ImageQuality { get; set; }
        public int? ImageResolutionWidth { get; set; }
        public int? ImageResolutionHeight { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? ConnectionString { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}