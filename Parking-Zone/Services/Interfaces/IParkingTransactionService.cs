using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingTransactionService
    {
        Task<IEnumerable<ParkingTransaction>> GetTransactionsAsync();
        Task<ParkingTransaction> GetTransactionByIdAsync(Guid id);
        Task<ParkingTransaction> CreateTransactionAsync(ParkingTransaction transaction);
        Task UpdateTransactionAsync(ParkingTransaction transaction);
        Task UpdateTransactionFeeAsync(Guid transactionId, decimal fee);
        Task<decimal> CalculateParkingFeeAsync(ParkingTransaction transaction);
        Task<IEnumerable<ParkingTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
