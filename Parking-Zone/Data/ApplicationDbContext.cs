using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Models;

namespace Parking_Zone.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ParkingZone> ParkingZones { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
