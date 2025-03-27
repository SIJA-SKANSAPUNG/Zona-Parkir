using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Models;
using Parking_Zone.Data;

namespace Parking_Zone.Services
{
    public class ParkingService : IParkingService
    {
        private readonly ILogger<ParkingService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IParkingFeeService _feeService;
        private readonly IParkingNotificationService _notificationService;

        public ParkingService(
            ILogger<ParkingService> logger,
            ApplicationDbContext context,
            IParkingFeeService feeService,
            IParkingNotificationService notificationService)
        {
            _logger = logger;
            _context = context;
            _feeService = feeService;
            _notificationService = notificationService;
        }

        public async Task<ParkingZone> GetParkingZoneByIdAsync(Guid id)
        {
            try
            {
                var zone = await _context.ParkingZones
                    .Include(z => z.ParkingSlots)
                    .FirstOrDefaultAsync(z => z.Id == id);

                if (zone == null)
                {
                    throw new KeyNotFoundException($"Parking zone {id} not found");
                }

                return zone;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving parking zone {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingZone>> GetAllParkingZonesAsync()
        {
            try
            {
                return await _context.ParkingZones
                    .Include(z => z.ParkingSlots)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all parking zones");
                throw;
            }
        }

        public async Task<ParkingSlot> GetParkingSlotByIdAsync(Guid id)
        {
            try
            {
                var slot = await _context.ParkingSlots
                    .Include(s => s.ParkingZone)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (slot == null)
                {
                    throw new KeyNotFoundException($"Parking slot {id} not found");
                }

                return slot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving parking slot {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingSlot>> GetAvailableSlotsAsync(Guid zoneId)
        {
            try
            {
                return await _context.ParkingSlots
                    .Include(s => s.ParkingZone)
                    .Where(s => s.ParkingZoneId == zoneId && !s.IsOccupied)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving available slots for zone {zoneId}");
                throw;
            }
        }

        public async Task<bool> IsParkingSlotAvailableAsync(Guid slotId)
        {
            try
            {
                var slot = await GetParkingSlotByIdAsync(slotId);
                return !slot.IsOccupied;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking availability for slot {slotId}");
                throw;
            }
        }

        public async Task<ParkingTransaction> CreateParkingTransactionAsync(VehicleEntry entry)
        {
            try
            {
                var transaction = new ParkingTransaction
                {
                    VehicleId = entry.VehicleId,
                    ParkingZoneId = entry.ParkingZoneId,
                    EntryTime = DateTime.UtcNow,
                    Status = "Active"
                };

                _context.ParkingTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyTransactionUpdated(entry.ParkingZoneId, transaction);
                _logger.LogInformation($"Created parking transaction for vehicle {entry.VehicleId} in zone {entry.ParkingZoneId}");

                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating parking transaction for vehicle {entry.VehicleId}");
                throw;
            }
        }

        public async Task<ParkingTransaction> GetTransactionByIdAsync(Guid id)
        {
            try
            {
                var transaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (transaction == null)
                {
                    throw new KeyNotFoundException($"Transaction {id} not found");
                }

                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transaction {id}");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                    .OrderByDescending(t => t.EntryTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transactions between {startDate} and {endDate}");
                throw;
            }
        }

        public async Task<decimal> CalculateParkingFeeAsync(Guid transactionId)
        {
            try
            {
                var transaction = await GetTransactionByIdAsync(transactionId);
                if (transaction.ExitTime == null)
                {
                    throw new InvalidOperationException("Cannot calculate fee for active transaction");
                }

                return await _feeService.CalculateFee(
                    transaction.EntryTime,
                    transaction.ExitTime.Value,
                    transaction.Vehicle.VehicleType,
                    transaction.ParkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating parking fee for transaction {transactionId}");
                throw;
            }
        }

        public async Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string status)
        {
            try
            {
                var transaction = await GetTransactionByIdAsync(transactionId);
                transaction.Status = status;

                if (status == "Completed")
                {
                    transaction.ExitTime = DateTime.UtcNow;
                    transaction.Amount = await CalculateParkingFeeAsync(transactionId);
                }

                _context.ParkingTransactions.Update(transaction);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyTransactionUpdated(transaction.ParkingZoneId, transaction);
                _logger.LogInformation($"Updated transaction {transactionId} status to {status}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating transaction {transactionId} status to {status}");
                return false;
            }
        }

        public async Task<bool> ValidateVehicleEntryAsync(string vehicleNumber, Guid gateId)
        {
            try
            {
                // Check if vehicle is already inside
                var activeTransaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.Vehicle.PlateNumber == vehicleNumber && t.ExitTime == null);

                if (activeTransaction != null)
                {
                    _logger.LogWarning($"Vehicle {vehicleNumber} is already inside the parking");
                    return false;
                }

                // Additional validation logic can be added here
                // For example, checking if the vehicle has a valid subscription or reservation

                _logger.LogInformation($"Vehicle {vehicleNumber} validated for entry at gate {gateId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating vehicle {vehicleNumber} entry at gate {gateId}");
                return false;
            }
        }

        public async Task<bool> ValidateVehicleExitAsync(string vehicleNumber, Guid gateId)
        {
            try
            {
                var activeTransaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.Vehicle.PlateNumber == vehicleNumber && t.ExitTime == null);

                if (activeTransaction == null)
                {
                    _logger.LogWarning($"No active transaction found for vehicle {vehicleNumber}");
                    return false;
                }

                // Additional validation logic can be added here
                // For example, checking if the parking fee has been paid

                _logger.LogInformation($"Vehicle {vehicleNumber} validated for exit at gate {gateId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating vehicle {vehicleNumber} exit at gate {gateId}");
                return false;
            }
        }

        public async Task<IEnumerable<ParkingTransaction>> GetActiveTransactionsAsync()
        {
            try
            {
                return await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .Where(t => t.ExitTime == null)
                    .OrderBy(t => t.EntryTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active transactions");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsByVehicleAsync(string vehicleNumber)
        {
            try
            {
                return await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .Where(t => t.Vehicle.PlateNumber == vehicleNumber)
                    .OrderByDescending(t => t.EntryTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transactions for vehicle {vehicleNumber}");
                throw;
            }
        }

        public async Task<bool> CancelTransactionAsync(Guid transactionId, string reason)
        {
            try
            {
                var transaction = await GetTransactionByIdAsync(transactionId);
                if (transaction.Status == "Completed")
                {
                    throw new InvalidOperationException("Cannot cancel a completed transaction");
                }

                transaction.Status = "Cancelled";
                transaction.ExitTime = DateTime.UtcNow;
                transaction.CancellationReason = reason;

                _context.ParkingTransactions.Update(transaction);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyTransactionUpdated(transaction.ParkingZoneId, transaction);
                _logger.LogInformation($"Cancelled transaction {transactionId} with reason: {reason}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling transaction {transactionId}");
                return false;
            }
        }

        public async Task<decimal> GetBaseRateAsync(int parkingSpotId)
        {
            try
            {
                var parkingSpot = await _context.ParkingSlots
                    .Include(ps => ps.ParkingZone)
                    .ThenInclude(pz => pz.VehicleTypes)
                    .FirstOrDefaultAsync(ps => ps.Id == parkingSpotId);

                if (parkingSpot == null)
                {
                    throw new KeyNotFoundException($"Parking spot {parkingSpotId} not found");
                }

                // Assuming the first vehicle type in the zone has the base rate
                var baseRate = parkingSpot.ParkingZone.VehicleTypes.FirstOrDefault()?.BaseRate 
                    ?? throw new InvalidOperationException($"No base rate found for parking spot {parkingSpotId}");

                return baseRate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving base rate for parking spot {parkingSpotId}");
                throw;
            }
        }
    }
} 