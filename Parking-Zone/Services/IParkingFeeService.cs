using Parking_Zone.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public interface IParkingFeeService
    {
        Task<decimal> CalculateFee(DateTime entryTime, DateTime exitTime, string vehicleType, Guid parkingZoneId);
        Task<decimal> GetBaseFee(string vehicleType, Guid parkingZoneId);
        Task UpdateFeeConfiguration(decimal baseFee, string vehicleType, Guid parkingZoneId);
        Task<IEnumerable<FeeConfiguration>> GetAllFeeConfigurations();
    }
} 