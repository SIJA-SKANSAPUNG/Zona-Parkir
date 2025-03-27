using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingService
    {
        Task<ParkingZone> GetParkingZoneByIdAsync(Guid id);
        Task<IEnumerable<ParkingZone>> GetAllParkingZonesAsync();
        Task<ParkingSlot> GetParkingSlotByIdAsync(Guid id);
        Task<IEnumerable<ParkingSlot>> GetAvailableSlotsAsync(Guid zoneId);
        Task<bool> IsParkingSlotAvailableAsync(Guid slotId);
        Task<ParkingTransaction> CreateParkingTransactionAsync(VehicleEntry entry);
        Task<ParkingTransaction> GetTransactionByIdAsync(Guid id);
        Task<IEnumerable<ParkingTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateParkingFeeAsync(Guid transactionId);
        Task<bool> UpdateTransactionStatusAsync(Guid transactionId, string status);
        Task<bool> ValidateVehicleEntryAsync(string vehicleNumber, Guid gateId);
        Task<bool> ValidateVehicleExitAsync(string vehicleNumber, Guid gateId);
        Task<IEnumerable<ParkingTransaction>> GetActiveTransactionsAsync();
        Task<IEnumerable<ParkingTransaction>> GetTransactionsByVehicleAsync(string vehicleNumber);
        Task<bool> CancelTransactionAsync(Guid transactionId, string reason);
    }
} 