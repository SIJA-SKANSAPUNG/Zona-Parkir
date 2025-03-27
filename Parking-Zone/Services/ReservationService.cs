using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Parking_Zone.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IParkingService _parkingService;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(
            ApplicationDbContext context, 
            IParkingService parkingService,
            ILogger<ReservationService> logger)
        {
            _context = context;
            _parkingService = parkingService;
            _logger = logger;
        }
        
        // Implementation of the methods required by ReservationController
        public IEnumerable<Reservation> GetByAppUserId(string userId)
        {
            return _context.Reservations
                .Include(r => r.ParkingSlot)
                .Include(r => r.ParkingSlot.ParkingZone)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartTime)
                .ToList();
        }
        
        public Reservation GetById(Guid reservationId)
        {
            return _context.Reservations
                .Include(r => r.ParkingSlot)
                .Include(r => r.ParkingSlot.ParkingZone)
                .FirstOrDefault(r => r.Id == reservationId);
        }
        
        public void Prolong(Reservation reservation, int extraHours)
        {
            if (reservation == null)
                throw new ArgumentNullException(nameof(reservation));
                
            // Add extra hours to the reservation duration
            reservation.Duration += extraHours;
            
            // Update the reservation in the database
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }

        public async Task<Reservation> CreateReservationAsync(
            string userId, 
            int spotId, 
            DateTime start, 
            DateTime end, 
            string vehicle)
        {
            try
            {
                // Validate parking spot exists
                var parkingSpot = await _context.ParkingSpaces
                    .FirstOrDefaultAsync(ps => ps.Id == spotId);

                if (parkingSpot == null)
                {
                    throw new KeyNotFoundException($"Parking spot with ID {spotId} not found.");
                }

                // Check for conflicting reservations
                var conflictingReservation = await _context.Reservations
                    .AnyAsync(r => r.ParkingSpotId == spotId &&
                                   r.Status != ReservationStatus.Cancelled &&
                                   r.Status != ReservationStatus.Expired &&
                                   ((start >= r.StartTime && start < r.EndTime) ||
                                    (end > r.StartTime && end <= r.EndTime) ||
                                    (start <= r.StartTime && end >= r.EndTime)));

                if (conflictingReservation)
                {
                    throw new InvalidOperationException("The selected parking spot is already reserved for the specified time.");
                }

                // Create reservation
                var reservation = new Reservation
                {
                    Id = Guid.NewGuid(),
                    ReservationNumber = GenerateReservationNumber(),
                    UserId = userId,
                    ParkingSpotId = spotId,
                    StartTime = start,
                    EndTime = end,
                    VehicleNumber = vehicle,
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created reservation {reservation.ReservationNumber}");
                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                throw;
            }
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.User)
                .Include(r => r.ParkingSpot)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Reservation>> GetUserReservationsAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.ParkingSpot)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetActiveReservationsAsync(int spotId)
        {
            var now = DateTime.UtcNow;
            return await _context.Reservations
                .Where(r => r.ParkingSpotId == spotId && 
                           r.StartTime <= now && 
                           r.EndTime >= now &&
                           r.Status == "Confirmed")
                .ToListAsync();
        }

        public async Task<bool> CancelReservationAsync(int id, string userId, string reason)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null || reservation.UserId != userId)
                    return false;

                reservation.Status = ReservationStatus.Cancelled.ToString();
                reservation.CancellationReason = reason;
                reservation.CancelledAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling reservation for ID {id}");
                throw;
            }
        }

        public async Task<bool> ConfirmReservationAsync(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                    return false;

                reservation.Status = ReservationStatus.Confirmed.ToString();
                reservation.ConfirmedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming reservation for ID {id}");
                throw;
            }
        }

        public async Task<bool> CompleteReservationAsync(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                    return false;

                reservation.Status = ReservationStatus.Completed.ToString();
                reservation.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing reservation for ID {id}");
                throw;
            }
        }

        public async Task<bool> ValidateReservationAsync(string userId, int spotId)
        {
            try
            {
                var now = DateTime.UtcNow;
                var hasActiveReservation = await _context.Reservations
                    .AnyAsync(r => r.UserId == userId && 
                                  r.ParkingSpotId == spotId && 
                                  r.StartTime <= now && 
                                  r.EndTime >= now &&
                                  r.Status == ReservationStatus.Confirmed.ToString());

                return hasActiveReservation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating reservation");
                throw;
            }
        }

        public async Task<decimal> CalculateReservationFeeAsync(int spotId, DateTime start, DateTime end)
        {
            try
            {
                var spot = await _context.ParkingSpaces.FindAsync(spotId);
                if (spot == null)
                    throw new ArgumentException("Invalid parking spot ID");

                var duration = end - start;
                var baseRate = await _parkingService.GetBaseRateAsync(spot.Type);
                var hours = Math.Ceiling(duration.TotalHours);

                return baseRate * (decimal)hours;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating reservation fee");
                throw;
            }
        }

        // Helper method to generate unique reservation number
        private string GenerateReservationNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomNum = new Random().Next(1000, 9999).ToString();
            return $"RES{timestamp}{randomNum}";
        }
    }
}
