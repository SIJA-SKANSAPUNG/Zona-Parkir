using System;
using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class ParkIRC
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Code { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public DateTime IssuedDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public Guid IssuedById { get; set; }
        public string IssuedByName { get; set; }
        
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        
        public decimal CreditAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        
        // Navigation properties as needed
        public virtual ApplicationUser IssuedBy { get; set; }
    }
} 