using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Enums;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IReservationService
    {
        Task<Reservation> CreateReservationAsync(string userId, int parkingZoneId, DateTime startTime, DateTime endTime, string vehiclePlateNumber);
        Task<Reservation> GetReservationByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId);
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync(int parkingZoneId);
        Task<bool> CancelReservationAsync(int id, string userId, string reason);
        Task<bool> ConfirmReservationAsync(int id);
        Task<bool> CompleteReservationAsync(int id);
        Task<bool> ValidateReservationAsync(string vehiclePlateNumber, int parkingZoneId);
        Task<decimal> CalculateReservationFeeAsync(int parkingZoneId, DateTime startTime, DateTime endTime);
    }
}
