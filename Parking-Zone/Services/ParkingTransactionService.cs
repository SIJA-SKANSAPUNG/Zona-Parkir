using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parking_Zone.Data;
using Parking_Zone.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public class ParkingTransactionService : IParkingTransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IParkingFeeService _feeService;
        private readonly ILogger<ParkingTransactionService> _logger;

        public ParkingTransactionService(
            ApplicationDbContext context,
            IParkingFeeService feeService,
            ILogger<ParkingTransactionService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _feeService = feeService ?? throw new ArgumentNullException(nameof(feeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ParkingTransaction> CreateTransactionAsync(Guid vehicleId, Guid parkingZoneId)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(vehicleId);
                var parkingZone = await _context.ParkingZones.FindAsync(parkingZoneId);

                if (vehicle == null)
                    throw new KeyNotFoundException($"Vehicle with ID {vehicleId} not found.");
                if (parkingZone == null)
                    throw new KeyNotFoundException($"Parking zone with ID {parkingZoneId} not found.");

                var transaction = new ParkingTransaction
                {
                    Id = Guid.NewGuid(),
                    VehicleId = vehicleId,
                    ParkingZoneId = parkingZoneId,
                    StartTime = DateTime.UtcNow,
                    Status = TransactionStatus.Active
                };

                _context.ParkingTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created parking transaction for vehicle {VehicleId}", vehicleId);
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating parking transaction for vehicle {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<ParkingTransaction> CompleteTransactionAsync(Guid transactionId, decimal amount)
        {
            try
            {
                var transaction = await _context.ParkingTransactions
                    .Include(t => t.Vehicle)
                    .FirstOrDefaultAsync(t => t.Id == transactionId);

                if (transaction == null)
                    throw new KeyNotFoundException($"Transaction with ID {transactionId} not found.");

                if (transaction.Status != TransactionStatus.Active)
                    throw new InvalidOperationException($"Transaction {transactionId} is not active.");

                transaction.EndTime = DateTime.UtcNow;
                transaction.Amount = amount;
                transaction.Status = TransactionStatus.Completed;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Completed parking transaction {TransactionId}", transactionId);
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing parking transaction {TransactionId}", transactionId);
                throw;
            }
        }

        public async Task<ParkingTransaction> GetTransactionByIdAsync(Guid id)
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.ParkingZone)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<ParkingTransaction>> GetAllTransactionsAsync()
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.ParkingZone)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingTransaction>> GetActiveTransactionsAsync()
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.ParkingZone)
                .Where(t => t.Status == TransactionStatus.Active)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsByVehicleAsync(Guid vehicleId)
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.ParkingZone)
                .Where(t => t.VehicleId == vehicleId)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .Include(t => t.ParkingZone)
                .Where(t => t.StartTime >= startDate && t.StartTime <= endDate)
                .OrderByDescending(t => t.StartTime)
                .ToListAsync();
        }

        public async Task<decimal> CalculateParkingFeeAsync(Guid transactionId)
        {
            var transaction = await GetTransactionByIdAsync(transactionId);
            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {transactionId} not found.");

            if (transaction.Status != TransactionStatus.Active)
                throw new InvalidOperationException($"Cannot calculate fee for non-active transaction {transactionId}.");

            var endTime = DateTime.UtcNow;
            var duration = endTime - transaction.StartTime;
            
            return await _feeService.CalculateFee(transaction.Vehicle.VehicleType, duration);
        }
    }
}
