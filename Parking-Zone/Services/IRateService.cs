using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IRateService
    {
        Task<Rate> GetRateByIdAsync(int id);
        Task<IEnumerable<Rate>> GetAllRatesAsync();
        Task<IEnumerable<Rate>> GetRatesByVehicleTypeAsync(string vehicleType);
        Task<IEnumerable<Rate>> GetRatesByZoneIdAsync(int zoneId);
        Task<Rate> CreateRateAsync(Rate rate);
        Task<Rate> UpdateRateAsync(Rate rate);
        Task<bool> DeleteRateAsync(int id);
        Task<decimal> CalculateRateAsync(string vehicleType, int zoneId, TimeSpan duration);
    }
} 