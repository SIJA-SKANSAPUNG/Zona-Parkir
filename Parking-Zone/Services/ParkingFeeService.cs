using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public class ParkingFeeService : IParkingFeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParkingFeeService> _logger;

        public ParkingFeeService(ApplicationDbContext context, ILogger<ParkingFeeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<decimal> CalculateFee(DateTime entryTime, DateTime exitTime, string vehicleType, Guid parkingZoneId)
        {
            try
            {
                var baseFee = await GetBaseFee(vehicleType, parkingZoneId);
                var duration = exitTime - entryTime;
                var hours = Math.Ceiling(duration.TotalHours);
                
                // Minimum 1 hour
                if (hours < 1) hours = 1;

                var totalFee = baseFee * (decimal)hours;
                
                _logger.LogInformation($"Calculated parking fee for vehicle type {vehicleType}: {totalFee:C} " +
                                     $"(Duration: {hours} hours, Base fee: {baseFee:C})");
                
                return totalFee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating parking fee for vehicle type {vehicleType}");
                throw;
            }
        }

        public async Task<decimal> GetBaseFee(string vehicleType, Guid parkingZoneId)
        {
            try
            {
                var feeConfig = await _context.Set<FeeConfiguration>()
                    .FirstOrDefaultAsync(f => f.VehicleType == vehicleType && f.ParkingZoneId == parkingZoneId);

                if (feeConfig == null)
                {
                    // Default fees if not configured
                    return vehicleType.ToLower() switch
                    {
                        "car" => 5000m,
                        "motorcycle" => 2000m,
                        "truck" => 10000m,
                        _ => 5000m
                    };
                }

                return feeConfig.BaseFee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting base fee for vehicle type {vehicleType}");
                throw;
            }
        }

        public async Task UpdateFeeConfiguration(decimal baseFee, string vehicleType, Guid parkingZoneId)
        {
            try
            {
                var feeConfig = await _context.Set<FeeConfiguration>()
                    .FirstOrDefaultAsync(f => f.VehicleType == vehicleType && f.ParkingZoneId == parkingZoneId);

                if (feeConfig == null)
                {
                    feeConfig = new FeeConfiguration
                    {
                        Id = Guid.NewGuid(),
                        VehicleType = vehicleType,
                        ParkingZoneId = parkingZoneId
                    };
                    _context.Add(feeConfig);
                }

                feeConfig.BaseFee = baseFee;
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"Updated parking fee configuration for {vehicleType}: {baseFee:C}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating fee configuration for vehicle type {vehicleType}");
                throw;
            }
        }

        public async Task<IEnumerable<FeeConfiguration>> GetAllFeeConfigurations()
        {
            try
            {
                return await _context.Set<FeeConfiguration>()
                    .Include(f => f.ParkingZone)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all fee configurations");
                throw;
            }
        }
    }
} 