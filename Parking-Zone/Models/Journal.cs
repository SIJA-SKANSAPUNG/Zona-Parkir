using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class Journal
    {
        public Guid Id { get; set; }
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        [Required]
        public string EventType { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = null!;
        
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        
        public Guid? EntityId { get; set; }
        public string? EntityType { get; set; }
        
        public string? Details { get; set; }
        public string? IPAddress { get; set; }

        // New properties for GateController
        public string? Action { get; set; }
        public Guid? OperatorId { get; set; }
        
        // Navigation properties (as needed)
        public virtual ApplicationUser? User { get; set; }
    }
}