using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Enums;
using Parking_Zone.Models;
using Parking_Zone.Repositories;
using Parking_Zone.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IParkingService _parkingService;

        public ReservationService(ApplicationDbContext context, IParkingService parkingService)
        {
            _context = context;
            _parkingService = parkingService;
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

        public async Task<Reservation> CreateReservationAsync(string userId, int spotId, DateTime start, DateTime end, string vehicle)
        {
            var reservation = new Reservation
            {
                UserId = userId,
                ParkingSpotId = spotId,
                StartTime = start,
                EndTime = end,
                VehicleNumber = vehicle,
                Status = "Pending"
            };

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
            return reservation;
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
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || reservation.UserId != userId)
                return false;

            reservation.Status = "Cancelled";
            reservation.CancellationReason = reason;
            reservation.CancelledAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return false;

            reservation.Status = "Confirmed";
            reservation.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return false;

            reservation.Status = "Completed";
            reservation.CompletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateReservationAsync(string userId, int spotId)
        {
            var now = DateTime.UtcNow;
            var hasActiveReservation = await _context.Reservations
                .AnyAsync(r => r.UserId == userId && 
                              r.ParkingSpotId == spotId && 
                              r.StartTime <= now && 
                              r.EndTime >= now &&
                              r.Status == "Confirmed");

            return hasActiveReservation;
        }

        public async Task<decimal> CalculateReservationFeeAsync(int spotId, DateTime start, DateTime end)
        {
            var spot = await _context.ParkingSpaces.FindAsync(spotId);
            if (spot == null)
                throw new ArgumentException("Invalid parking spot ID");

            var duration = end - start;
            var baseRate = await _parkingService.GetBaseRateAsync(spot.Type);
            var hours = Math.Ceiling(duration.TotalHours);

            return baseRate * (decimal)hours;
        }
    }
}
