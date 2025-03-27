using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class EntryGate
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Location { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string Status { get; set; } = "Operational";
        
        public DateTime? LastActivity { get; set; }
        
        public string IPAddress { get; set; }
        
        public string SerialNumber { get; set; }
        
        public string Notes { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Add IsOnline and IsOpen properties
        public bool IsOnline { get; set; } = false;
        public bool IsOpen { get; set; } = false;
        
        // Navigation properties
        public virtual ICollection<ParkingTransaction> Transactions { get; set; } = new List<ParkingTransaction>();
        public virtual ICollection<GateOperation> Operations { get; set; } = new List<GateOperation>();
    }
}