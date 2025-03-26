using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class Settings
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        [StringLength(50)]
        public string Group { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public bool IsSystem { get; set; }

        public bool IsEncrypted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual AppUser UpdatedBy { get; set; }
        public string UpdatedById { get; set; }
    }
} 