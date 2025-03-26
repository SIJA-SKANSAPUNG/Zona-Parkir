using Parking_Zone.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public interface IParkingTransactionService
    {
        Task<ParkingTransaction> CreateTransactionAsync(Guid vehicleId, Guid parkingZoneId);
        Task<ParkingTransaction> CompleteTransactionAsync(Guid transactionId, decimal amount);
        Task<ParkingTransaction> GetTransactionByIdAsync(Guid id);
        Task<IEnumerable<ParkingTransaction>> GetAllTransactionsAsync();
        Task<IEnumerable<ParkingTransaction>> GetActiveTransactionsAsync();
        Task<IEnumerable<ParkingTransaction>> GetTransactionsByVehicleAsync(Guid vehicleId);
        Task<IEnumerable<ParkingTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateParkingFeeAsync(Guid transactionId);
    }
}
