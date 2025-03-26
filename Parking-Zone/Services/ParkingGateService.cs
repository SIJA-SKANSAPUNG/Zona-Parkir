using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parking_Zone.Data;
using Parking_Zone.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public class ParkingGateService : IParkingGateService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParkingGateService> _logger;
        private readonly IVehicleService _vehicleService;

        public ParkingGateService(ApplicationDbContext context, ILogger<ParkingGateService> logger, IVehicleService vehicleService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vehicleService = vehicleService;
        }

        public async Task<ParkingGate> GetGateByIdAsync(Guid id)
        {
            try
            {
                return await _context.ParkingGates
                    .Include(g => g.ParkingZone)
                    .FirstOrDefaultAsync(g => g.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving gate {GateId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<ParkingGate>> GetAllGatesAsync()
        {
            try
            {
                return await _context.ParkingGates
                    .Include(g => g.ParkingZone)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all gates");
                throw;
            }
        }

        public async Task<bool> OpenGateAsync(int gateId)
        {
            var gate = await _context.ParkingGates.FindAsync(gateId);
            if (gate == null) return false;

            gate.IsOpen = true;
            gate.LastOperationTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CloseGateAsync(int gateId)
        {
            var gate = await _context.ParkingGates.FindAsync(gateId);
            if (gate == null) return false;

            gate.IsOpen = false;
            gate.LastOperationTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsGateOpenAsync(int gateId)
        {
            var gate = await _context.ParkingGates.FindAsync(gateId);
            return gate?.IsOpen ?? false;
        }

        public async Task<ParkingGate> GetGateStatusAsync(int gateId)
        {
            return await _context.ParkingGates
                .Include(g => g.ParkingZone)
                .FirstOrDefaultAsync(g => g.Id == gateId);
        }

        public async Task<bool> ValidateEntryAsync(string vehiclePlateNumber, int gateId)
        {
            var gate = await _context.ParkingGates
                .Include(g => g.ParkingZone)
                .FirstOrDefaultAsync(g => g.Id == gateId);

            if (gate == null || !gate.IsOperational) return false;

            var vehicle = await _vehicleService.GetVehicleByPlateNumberAsync(vehiclePlateNumber);
            if (vehicle == null) return false;

            // Check if vehicle has active reservation or subscription
            var hasActiveReservation = await _context.Reservations
                .AnyAsync(r => r.VehiclePlateNumber == vehiclePlateNumber && 
                              r.Status == ReservationStatus.Confirmed &&
                              r.StartTime <= DateTime.UtcNow &&
                              r.EndTime >= DateTime.UtcNow);

            return hasActiveReservation;
        }

        public async Task<bool> ValidateExitAsync(string vehiclePlateNumber, int gateId)
        {
            var gate = await _context.ParkingGates
                .Include(g => g.ParkingZone)
                .FirstOrDefaultAsync(g => g.Id == gateId);

            if (gate == null || !gate.IsOperational) return false;

            var transaction = await _context.ParkingTransactions
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Vehicle.PlateNumber == vehiclePlateNumber && 
                                        t.ExitTime == null);

            return transaction != null;
        }

        public async Task LogGateOperationAsync(int gateId, string operation, string userId)
        {
            var log = new ParkingTransaction
            {
                GateId = gateId,
                OperatorId = userId,
                OperationType = operation,
                Timestamp = DateTime.UtcNow
            };

            _context.ParkingTransactions.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
