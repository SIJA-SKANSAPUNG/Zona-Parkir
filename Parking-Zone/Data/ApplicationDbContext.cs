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
        public DbSet<CameraSettings> CameraSettings { get; set; } = null!;
        public DbSet<PrinterConfig> PrinterConfigs { get; set; } = null!;
        public DbSet<SiteSettings> SiteSettings { get; set; } = null!;
        public DbSet<VehicleEntry> VehicleEntries { get; set; } = null!;
        public DbSet<VehicleExit> VehicleExits { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for Vehicle
            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasMany(v => v.ParkingTransactions)
                    .WithOne(t => t.Vehicle)
                    .HasForeignKey(t => t.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.VehicleEntries)
                    .WithOne(ve => ve.Vehicle)
                    .HasForeignKey(ve => ve.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(v => v.VehicleExits)
                    .WithOne(vx => vx.Vehicle)
                    .HasForeignKey(vx => vx.VehicleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure relationships for ParkingGate
            modelBuilder.Entity<ParkingGate>(entity =>
            {
                entity.HasOne(g => g.ParkingZone)
                    .WithMany()
                    .HasForeignKey(g => g.ParkingZoneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Configure relationships for ParkingTransaction
            modelBuilder.Entity<ParkingTransaction>(entity =>
            {
                entity.HasOne(t => t.ParkingZone)
                    .WithMany()
                    .HasForeignKey(t => t.ParkingZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CameraSettings
            modelBuilder.Entity<CameraSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ProfileName).IsRequired();
                entity.Property(e => e.LightingCondition).HasDefaultValue("Normal");
            });

            // Configure PrinterConfig
            modelBuilder.Entity<PrinterConfig>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Port).IsRequired();
                entity.Property(e => e.ConnectionType).HasDefaultValue("Serial");
                entity.Property(e => e.Status).HasDefaultValue("Offline");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.LastChecked).IsRequired();
            });

            // Configure SiteSettings
            modelBuilder.Entity<SiteSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SiteName).IsRequired();
                entity.Property(e => e.ThemeColor).HasDefaultValue("#007bff");
                entity.Property(e => e.ShowLogo).HasDefaultValue(true);
                entity.Property(e => e.EnableNotifications).HasDefaultValue(true);
                entity.Property(e => e.LastUpdated).IsRequired();
                entity.Property(e => e.UpdatedBy).IsRequired();
            });

            // Configure VehicleEntry
            modelBuilder.Entity<VehicleEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EntryTime).IsRequired();
            });

            // Configure VehicleExit
            modelBuilder.Entity<VehicleExit>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ExitTime).IsRequired();
                entity.HasOne(vx => vx.Transaction)
                    .WithMany()
                    .HasForeignKey(vx => vx.TransactionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

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

            // Seed default SiteSettings
            modelBuilder.Entity<SiteSettings>().HasData(
                new SiteSettings
                {
                    Id = Guid.NewGuid(),
                    SiteName = "Parking Zone Management System",
                    ThemeColor = "#007bff",
                    ShowLogo = true,
                    EnableNotifications = true,
                    LastUpdated = DateTime.UtcNow,
                    UpdatedBy = "system"
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
                Id = "c5f10bb4-851e-4e6f-b832-da4c4f73c553",
                UserName = "admin@parking.com",
                NormalizedUserName = "ADMIN@PARKING.COM",
                Email = "admin@parking.com",
                NormalizedEmail = "ADMIN@PARKING.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = "System",
                LastName = "Administrator"
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            modelBuilder.Entity<AppUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id
                }
            );
        }
    }
}
