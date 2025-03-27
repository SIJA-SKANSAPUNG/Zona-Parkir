using Parking_Zone.Services;
using Parking_Zone.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class ParkingTransactionServiceExtensions
    {
        public static async Task<ParkingTransaction> CreateTransactionAsync(
            this IParkingTransactionService service, 
            VehicleEntry entry, 
            VehicleExit exit)
        {
            return await service.CreateTransactionAsync(
                entry.LicensePlate, 
                entry.EntryTime, 
                exit.ExitTime
            );
        }

        public static async Task<List<ParkingTransaction>> GetTransactionsByVehicleAsync(
            this IParkingTransactionService service, 
            string vehiclePlate)
        {
            return await service.GetTransactionsByVehicleAsync(vehiclePlate);
        }

        public static async Task<List<ParkingTransaction>> GetActiveTransactionsAsync(
            this IParkingTransactionService service)
        {
            return await service.GetActiveTransactionsAsync();
        }

        public static async Task<List<ParkingTransaction>> GetAllTransactionsAsync(
            this IParkingTransactionService service)
        {
            return await service.GetAllTransactionsAsync();
        }

        public static async Task<ParkingTransaction> CompleteTransactionAsync(
            this IParkingTransactionService service, 
            Guid transactionId)
        {
            return await service.CompleteTransactionAsync(transactionId);
        }

        public static string GetVehicleNumber(this ParkingTransaction transaction)
        {
            return transaction.VehicleLicensePlate;
        }

        public static Guid GetGateId(this ParkingTransaction transaction)
        {
            return transaction.EntryGateId;
        }
    }
}
