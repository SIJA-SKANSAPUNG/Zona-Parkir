namespace Parking_Zone.Models
{
    public class ParkingZone
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfEstablishment { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<ParkingSlot> ParkingSlots { get; set; } = new List<ParkingSlot>();
    }
}