using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Models;
using Parking_Zone.Data;

namespace Parking_Zone.Services
{
    public class ParkingFeeService : IParkingFeeService
    {
        private readonly ILogger<ParkingFeeService> _logger;
        private readonly ApplicationDbContext _context;

        public ParkingFeeService(
            ILogger<ParkingFeeService> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<decimal> CalculateFee(DateTime entryTime, DateTime exitTime, string vehicleType, Guid parkingZoneId)
        {
            try
            {
                var feeConfig = await _context.FeeConfigurations
                    .FirstOrDefaultAsync(f => f.VehicleType == vehicleType && f.ParkingZoneId == parkingZoneId);

                if (feeConfig == null)
                {
                    throw new InvalidOperationException($"No fee configuration found for vehicle type {vehicleType} in parking zone {parkingZoneId}");
                }

                var duration = exitTime - entryTime;
                var hours = Math.Ceiling(duration.TotalHours);
                var fee = feeConfig.BaseFee * (decimal)hours;

                _logger.LogInformation($"Calculated fee for {vehicleType} in zone {parkingZoneId}: {fee:C} for {hours} hours");
                return fee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating fee for {vehicleType} in zone {parkingZoneId}");
                throw;
            }
        }

        public async Task<decimal> GetBaseFee(string vehicleType, Guid parkingZoneId)
        {
            try
            {
                var feeConfig = await _context.FeeConfigurations
                    .FirstOrDefaultAsync(f => f.VehicleType == vehicleType && f.ParkingZoneId == parkingZoneId);

                if (feeConfig == null)
                {
                    throw new InvalidOperationException($"No fee configuration found for vehicle type {vehicleType} in parking zone {parkingZoneId}");
                }

                return feeConfig.BaseFee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving base fee for {vehicleType} in zone {parkingZoneId}");
                throw;
            }
        }

        public async Task UpdateFeeConfiguration(decimal baseFee, string vehicleType, Guid parkingZoneId)
        {
            try
            {
                var feeConfig = await _context.FeeConfigurations
                    .FirstOrDefaultAsync(f => f.VehicleType == vehicleType && f.ParkingZoneId == parkingZoneId);

                if (feeConfig == null)
                {
                    feeConfig = new FeeConfiguration
                    {
                        VehicleType = vehicleType,
                        ParkingZoneId = parkingZoneId,
                        BaseFee = baseFee
                    };
                    _context.FeeConfigurations.Add(feeConfig);
                }
                else
                {
                    feeConfig.BaseFee = baseFee;
                    _context.FeeConfigurations.Update(feeConfig);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated fee configuration for {vehicleType} in zone {parkingZoneId}: {baseFee:C}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating fee configuration for {vehicleType} in zone {parkingZoneId}");
                throw;
            }
        }

        public async Task<IEnumerable<FeeConfiguration>> GetAllFeeConfigurations()
        {
            try
            {
                return await _context.FeeConfigurations
                    .Include(f => f.ParkingZone)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all fee configurations");
                throw;
            }
        }
    }
} 