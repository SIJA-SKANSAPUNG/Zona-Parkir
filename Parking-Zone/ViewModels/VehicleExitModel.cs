using System;

namespace Parking_Zone.ViewModels
{
    public class VehicleExitModel : BaseViewModel
    {
        public string LicensePlate { get; set; } = null!;
        public VehicleType VehicleType { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public TimeSpan ParkingDuration { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid? OperatorId { get; set; }
    }
}
