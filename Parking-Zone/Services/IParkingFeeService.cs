using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingFeeService
    {
        Task<decimal> CalculateFee(DateTime entryTime, DateTime exitTime, string vehicleType, Guid parkingZoneId);
        Task<decimal> GetBaseFee(string vehicleType, Guid parkingZoneId);
        Task UpdateFeeConfiguration(decimal baseFee, string vehicleType, Guid parkingZoneId);
        Task<IEnumerable<FeeConfiguration>> GetAllFeeConfigurations();
    }

    public class FeeConfiguration
    {
        public Guid Id { get; set; }
        public string VehicleType { get; set; }
        public decimal BaseFee { get; set; }
        public Guid ParkingZoneId { get; set; }
        public virtual ParkingZone ParkingZone { get; set; }
    }
} 