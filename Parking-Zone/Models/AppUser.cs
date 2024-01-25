using Microsoft.AspNetCore.Identity;

namespace Parking_Zone.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
