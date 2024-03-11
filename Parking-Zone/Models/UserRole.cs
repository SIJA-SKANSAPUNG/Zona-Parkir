namespace Parking_Zone.Models
{
    public class UserRole
    {
        public Guid Id { get; set; }
        public virtual AppUser AppUser { get; set; }
        public virtual Role Role { get; set; }
    }
}
