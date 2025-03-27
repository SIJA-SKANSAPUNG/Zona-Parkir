using Parking_Zone.Services;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class RateServiceExtensions
    {
        public static async Task<decimal> GetRateForVehicleTypeAsync(
            this IRateService service, 
            int vehicleTypeId)
        {
            return await service.GetRateForVehicleTypeAsync(vehicleTypeId);
        }

        public static decimal CalculateFee(
            this IRateService service, 
            decimal rate, 
            TimeSpan duration)
        {
            return service.CalculateFee(rate, duration);
        }
    }
}
