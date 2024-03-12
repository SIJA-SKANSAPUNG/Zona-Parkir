using Microsoft.AspNetCore.Identity;
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
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var adminRole = new IdentityRole { Id = "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de", Name = "Admin" };
            var userRole = new IdentityRole { Id = "d47b3c1e-1310-409d-b893-0a662a64c35d", Name = "User" };

            modelBuilder.Entity<IdentityRole>().HasData(
                adminRole,
                userRole
            );

            var hasher = new PasswordHasher<AppUser>();

            var adminUser = new AppUser
            {
                Id = "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                UserName = "admin@admin.com",
                Email = "admin@admin.com",
                FullName = "Adminov Admin"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

            var userUser = new AppUser
            {
                Id = "d47b3c1e-1310-409d-b893-0a662a64c35d",
                UserName = "user@user.com",
                Email = "user@user.com",
                FullName = "Userov User"
            };
            userUser.PasswordHash = hasher.HashPassword(userUser, "user1234");

            // Add users and roles
            modelBuilder.Entity<AppUser>().HasData(
                adminUser,
                userUser
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRole.Id },
                new IdentityUserRole<string> { UserId = userUser.Id, RoleId = userRole.Id }
            );
        }
    }
}
