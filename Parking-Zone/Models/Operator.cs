using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Parking_Zone.Models
{
    public class Operator : AppUser
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeId { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }
        
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<ParkingGate> AssignedGates { get; set; }

        public Operator()
        {
            Shifts = new HashSet<Shift>();
            AssignedGates = new HashSet<ParkingGate>();
        }
    }
}