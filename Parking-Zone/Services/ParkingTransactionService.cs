using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parking_Zone.Data;
using Parking_Zone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public class ParkingTransactionService : IParkingTransactionService
    {
        private readonly ILogger<ParkingTransactionService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRateService _rateService;
        private readonly IParkingFeeService _feeService;
        private readonly IParkingNotificationService _notificationService;

        public ParkingTransactionService(
            ILogger<ParkingTransactionService> logger,
            ApplicationDbContext context,
            IRateService rateService,
            IParkingFeeService feeService,
            IParkingNotificationService notificationService)
        {
            _logger = logger;
            _context = context;
            _rateService = rateService;
            _feeService = feeService;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsAsync()
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.EntryOperator)
                .Include(t => t.ExitOperator)
                .ToListAsync();
        }

        public async Task<ParkingTransaction> GetTransactionByIdAsync(Guid id)
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.EntryOperator)
                .Include(t => t.ExitOperator)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ParkingTransaction> CreateTransactionAsync(Guid vehicleId, Guid parkingZoneId)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(vehicleId);
                if (vehicle == null)
                {
                    throw new KeyNotFoundException($"Vehicle {vehicleId} not found");
                }

                var parkingZone = await _context.ParkingZones.FindAsync(parkingZoneId);
                if (parkingZone == null)
                {
                    throw new KeyNotFoundException($"Parking zone {parkingZoneId} not found");
                }

                var transaction = new ParkingTransaction
                {
                    VehicleId = vehicleId,
                    ParkingZoneId = parkingZoneId,
                    EntryTime = DateTime.UtcNow,
                    Status = "Active"
                };

                _context.ParkingTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyTransactionUpdated(parkingZoneId, transaction);
                _logger.LogInformation($"Created transaction for vehicle {vehicleId} in zone {parkingZoneId}");

                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating transaction for vehicle {vehicleId}");
                throw;
            }
        }

        public async Task<ParkingTransaction> CreateTransactionAsync(ParkingTransaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.TransactionNumber = GenerateTransactionNumber();

            _context.ParkingTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task UpdateTransactionAsync(ParkingTransaction transaction)
        {
            _context.ParkingTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransactionFeeAsync(Guid transactionId, decimal fee)
        {
            var transaction = await _context.ParkingTransactions.FindAsync(transactionId);
            if (transaction != null)
            {
                transaction.ParkingFee = fee;
                await _context.SaveChangesAsync();
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

        public async Task<decimal> CalculateParkingFeeAsync(ParkingTransaction transaction)
        {
            if (transaction.EntryTime == default || transaction.ExitTime == null)
                return 0;

            var duration = transaction.ExitTime.Value - transaction.EntryTime;
            var rate = await _rateService.GetRateForVehicleTypeAsync(transaction.VehicleType);

            return _rateService.CalculateFee(rate, duration);
        }

        public async Task<ParkingTransaction> CompleteTransactionAsync(Guid transactionId, decimal amount)
        {
            try
            {
                var transaction = await GetTransactionByIdAsync(transactionId);
                if (transaction.Status == "Completed")
                {
                    throw new InvalidOperationException($"Transaction {transactionId} is already completed");
                }

                transaction.Status = "Completed";
                transaction.ExitTime = DateTime.UtcNow;
                transaction.Amount = amount;
                transaction.IsPaid = true;

                _context.ParkingTransactions.Update(transaction);
                await _context.SaveChangesAsync();

                await _notificationService.NotifyTransactionUpdated(transaction.ParkingZoneId, transaction);
                _logger.LogInformation($"Completed transaction {transactionId} with amount {amount:C}");

                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing transaction {transactionId}");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ParkingTransactions
                .Where(t => t.EntryTime >= startDate && t.EntryTime <= endDate)
                .Include(t => t.Vehicle)
                .Include(t => t.EntryOperator)
                .Include(t => t.ExitOperator)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingTransaction>> GetAllTransactionsAsync()
        {
            try
            {
                return await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .OrderByDescending(t => t.EntryTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all transactions");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingTransaction>> GetActiveTransactionsAsync()
        {
            try
            {
                return await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .Where(t => t.Status == "Active")
                    .OrderBy(t => t.EntryTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active transactions");
                throw;
            }
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsByVehicleAsync(Guid vehicleId)
        {
            try
            {
                return await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .Include(t => t.ParkingZone)
                    .Where(t => t.VehicleId == vehicleId)
                    .OrderByDescending(t => t.EntryTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transactions for vehicle {vehicleId}");
                throw;
            }
        }

        private string GenerateTransactionNumber()
        {
            // Generate a unique transaction number
            return $"TRX-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
    }
}
