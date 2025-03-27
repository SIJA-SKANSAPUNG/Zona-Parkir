using System.ComponentModel.DataAnnotations;

namespace Parking_Zone.Models
{
    public class VehicleType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public decimal BaseRate { get; set; }

        [Required]
        public decimal AdditionalHourlyRate { get; set; }

        public bool IsActive { get; set; } = true;

        public string Description { get; set; }
    }
}
