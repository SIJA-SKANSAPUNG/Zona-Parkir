using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Models;

namespace Parking_Zone.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Parking Management
        public DbSet<ParkingZone> ParkingZones { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<ParkingTransaction> ParkingTransactions { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ParkingTicket> ParkingTickets { get; set; }
        public DbSet<VehicleEntry> VehicleEntries { get; set; }
        public DbSet<VehicleExit> VehicleExits { get; set; }
        public DbSet<VehicleEntryRequest> VehicleEntryRequests { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Journal> Journals { get; set; }

        // Gates and Operations
        public DbSet<ParkingGate> ParkingGates { get; set; }
        public DbSet<EntryGate> EntryGates { get; set; }
        public DbSet<ExitGate> ExitGates { get; set; }
        public DbSet<GateOperation> GateOperations { get; set; }

        // Additional entities mentioned in error messages
        public DbSet<ParkIRC> ParkIRCs { get; set; }
        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Operator> Operators { get; set; }

        // Rates and Fees
        public DbSet<Rate> Rates { get; set; }
        public DbSet<FeeConfiguration> FeeConfigurations { get; set; }

        // Hardware
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<Scanner> Scanners { get; set; }
        public DbSet<CameraConfiguration> CameraConfigurations { get; set; }
        public DbSet<PrinterConfiguration> PrinterConfigurations { get; set; }

        // Settings and Operations
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<Settings> Settings { get; set; }

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

            // Configure relationships for ParkingZone
            modelBuilder.Entity<ParkingZone>(entity =>
            {
                entity.HasMany(z => z.ParkingSlots)
                    .WithOne(s => s.ParkingZone)
                    .HasForeignKey(s => s.ParkingZoneId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(z => z.Gates)
                    .WithOne(g => g.ParkingZone)
                    .HasForeignKey(g => g.ParkingZoneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(z => z.Rates)
                    .WithOne(r => r.Zone)
                    .HasForeignKey(r => r.ZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure relationships for ParkingGate
            modelBuilder.Entity<ParkingGate>(entity =>
            {
                entity.HasOne(g => g.ParkingZone)
                    .WithMany(z => z.Gates)
                    .HasForeignKey(g => g.ParkingZoneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(g => g.Operations)
                    .WithOne(o => o.Gate)
                    .HasForeignKey(o => o.GateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // Configure relationships for ParkingTransaction
            modelBuilder.Entity<ParkingTransaction>(entity =>
            {
                entity.HasOne(t => t.ParkingZone)
                    .WithMany(z => z.ParkingTransactions)
                    .HasForeignKey(t => t.ParkingZoneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure relationships for ParkingSlot
            modelBuilder.Entity<ParkingSlot>(entity =>
            {
                entity.HasMany(s => s.Reservations)
                    .WithOne()
                    .HasForeignKey("ParkingSlotId")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(s => s.ParkingTransactions)
                    .WithOne()
                    .HasForeignKey("ParkingSlotId")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CameraConfiguration
            modelBuilder.Entity<CameraConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IpAddress).IsRequired();
                entity.Property(e => e.Port).IsRequired();
            });

            // Configure PrinterConfiguration
            modelBuilder.Entity<PrinterConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PortName).IsRequired();
            });
        }
    }
}
