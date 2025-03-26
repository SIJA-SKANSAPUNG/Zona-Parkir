using System;
using System.Collections.Generic;

namespace Parking_Zone.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; } = null!;
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public byte[]? PhotoEntry { get; set; }
        public byte[]? PhotoExit { get; set; }
        public string VehicleType { get; set; } = null!;
        public string? TicketBarcode { get; set; }
        public bool IsInside { get; set; }
        public decimal FeeAmount { get; set; }
        public bool IsPaid { get; set; }

        // Navigation properties
        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; }
        public virtual ICollection<VehicleEntry> VehicleEntries { get; set; }
        public virtual ICollection<VehicleExit> VehicleExits { get; set; }

        public Vehicle()
        {
            ParkingTransactions = new HashSet<ParkingTransaction>();
            VehicleEntries = new HashSet<VehicleEntry>();
            VehicleExits = new HashSet<VehicleExit>();
        }
    }
}