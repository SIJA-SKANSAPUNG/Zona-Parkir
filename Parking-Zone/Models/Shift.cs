using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Parking_Zone.Models
{
    public class Shift
    {
        public Guid Id { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public bool IsActive { get; set; }
        
        public Guid? OperatorId { get; set; }
        public string OperatorName { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal InitialCashAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalCashAmount { get; set; }
        
        public decimal? TotalRevenue { get; set; }
        
        public int? TotalTransactions { get; set; }
        
        public string Notes { get; set; }
        
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual Operator Operator { get; set; }

        public bool IsTimeInShift(DateTime checkTime)
        {
            return StartTime.HasValue && EndTime.HasValue &&
                   checkTime >= StartTime.Value && checkTime <= EndTime.Value;
        }

        public DateTime? Date => StartTime?.Date;
    }
}