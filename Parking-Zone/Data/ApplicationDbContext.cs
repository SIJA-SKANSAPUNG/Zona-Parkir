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
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ParkingGate> ParkingGates { get; set; }
        public DbSet<ParkingTransaction> ParkingTransactions { get; set; }
        public DbSet<FeeConfiguration> FeeConfigurations { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<Scanner> Scanners { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ParkingTicket> ParkingTickets { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for Vehicle
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.ParkingTransactions)
                .WithOne(t => t.Vehicle)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure relationships for ParkingGate
            modelBuilder.Entity<ParkingGate>()
                .HasOne(g => g.ParkingZone)
                .WithMany()
                .HasForeignKey(g => g.ParkingZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for ParkingTransaction
            modelBuilder.Entity<ParkingTransaction>()
                .HasOne(t => t.ParkingZone)
                .WithMany()
                .HasForeignKey(t => t.ParkingZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed ParkingZone data
            var mainZone = new ParkingZone
            {
                Id = Guid.Parse("f1b6c91a-5c6d-4c4f-b2b1-8f1c6d9c8b7a"),
                Name = "Main Parking Zone",
                Address = "Jl. Utama No. 1",
                Capacity = 100,
                IsActive = true
            };

            modelBuilder.Entity<ParkingZone>().HasData(mainZone);

            // Seed FeeConfiguration data
            modelBuilder.Entity<FeeConfiguration>().HasData(
                new FeeConfiguration
                {
                    Id = Guid.NewGuid(),
                    VehicleType = "car",
                    BaseFee = 5000m,
                    ParkingZoneId = mainZone.Id
                },
                new FeeConfiguration
                {
                    Id = Guid.NewGuid(),
                    VehicleType = "motorcycle",
                    BaseFee = 2000m,
                    ParkingZoneId = mainZone.Id
                },
                new FeeConfiguration
                {
                    Id = Guid.NewGuid(),
                    VehicleType = "truck",
                    BaseFee = 10000m,
                    ParkingZoneId = mainZone.Id
                }
            );

            // Existing seed data
            var adminRole = new IdentityRole { Id = "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de", Name = "Admin", NormalizedName = "ADMIN" };
            var userRole = new IdentityRole { Id = "d47b3c1e-1310-409d-b893-0a662a64c35d", Name = "User", NormalizedName = "USER" };

            modelBuilder.Entity<IdentityRole>().HasData(
                adminRole,
                userRole
            );

            var hasher = new PasswordHasher<AppUser>();

            var adminUser = new AppUser
            {
                Id = "9a535a8f-dccf-4a1d-a25d-d9c3bb4803de",
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                FullName = "Adminov Admin"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

            var userUser = new AppUser
            {
                Id = "d47b3c1e-1310-409d-b893-0a662a64c35d",
                UserName = "user@user.com",
                NormalizedUserName = "USER@USER.COM",
                Email = "user@user.com",
                NormalizedEmail = "USER@USER.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
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

            // Configure relationships for Camera
            modelBuilder.Entity<Camera>()
                .HasOne(c => c.Gate)
                .WithMany(g => g.Cameras)
                .HasForeignKey(c => c.GateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationships for Printer
            modelBuilder.Entity<Printer>()
                .HasOne(p => p.Gate)
                .WithMany(g => g.Printers)
                .HasForeignKey(p => p.GateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationships for Scanner
            modelBuilder.Entity<Scanner>()
                .HasOne(s => s.Gate)
                .WithMany(g => g.Scanners)
                .HasForeignKey(s => s.GateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure relationships for Operator
            modelBuilder.Entity<Operator>()
                .HasOne(o => o.User)
                .WithOne()
                .HasForeignKey<Operator>(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Operator>()
                .HasMany(o => o.AssignedGates)
                .WithMany()
                .UsingEntity(j => j.ToTable("OperatorGates"));

            // Configure relationships for Shift
            modelBuilder.Entity<Shift>()
                .HasOne(s => s.Operator)
                .WithMany(o => o.Shifts)
                .HasForeignKey(s => s.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shift>()
                .HasOne(s => s.Gate)
                .WithMany()
                .HasForeignKey(s => s.GateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for ParkingTicket
            modelBuilder.Entity<ParkingTicket>()
                .HasOne(t => t.Transaction)
                .WithOne()
                .HasForeignKey<ParkingTicket>(t => t.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParkingTicket>()
                .HasOne(t => t.IssuedBy)
                .WithMany()
                .HasForeignKey(t => t.IssuedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParkingTicket>()
                .HasOne(t => t.VoidedBy)
                .WithMany()
                .HasForeignKey(t => t.VoidedById)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ParkingTicket>()
                .HasOne(t => t.Gate)
                .WithMany()
                .HasForeignKey(t => t.GateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for Settings
            modelBuilder.Entity<Settings>()
                .HasOne(s => s.UpdatedBy)
                .WithMany()
                .HasForeignKey(s => s.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
