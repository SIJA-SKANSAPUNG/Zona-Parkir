using System;

namespace Parking_Zone.Models
{
    public class Vehicle
    {
        public Guid Id { get; set; }
        public string PlateNumber { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public byte[] PhotoEntry { get; set; }
        public byte[] PhotoExit { get; set; }
        public string VehicleType { get; set; }
        public string TicketBarcode { get; set; }
        public bool IsInside { get; set; }
        public decimal FeeAmount { get; set; }
        public bool IsPaid { get; set; }

        // Navigation properties
        public virtual ICollection<ParkingTransaction> ParkingTransactions { get; set; }

        public Vehicle()
        {
            ParkingTransactions = new HashSet<ParkingTransaction>();
        }
    }
} 