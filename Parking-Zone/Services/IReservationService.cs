using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Enums;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IReservationService
    {
        Task<Reservation> CreateReservationAsync(string userId, int spotId, DateTime start, DateTime end, string vehicle);
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId);
        Task<IEnumerable<Reservation>> GetActiveReservationsAsync(int spotId);
        Task<bool> CancelReservationAsync(int id, string userId, string reason);
        Task<bool> ConfirmReservationAsync(int id);
        Task<bool> CompleteReservationAsync(int id);
        Task<bool> ValidateReservationAsync(string userId, int spotId);
        Task<decimal> CalculateReservationFeeAsync(int spotId, DateTime start, DateTime end);
        
        // Additional methods needed by ReservationController
        IEnumerable<Reservation> GetByAppUserId(string userId);
        Reservation GetById(Guid reservationId);
        void Prolong(Reservation reservation, int extraHours);
    }
}
