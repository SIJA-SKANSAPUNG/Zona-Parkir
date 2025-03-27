using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public class ParkingGateService : IParkingGateService
    {
        private readonly ApplicationDbContext _context;

        public ParkingGateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ParkingGate>> GetAllEntryGatesAsync()
        {
            return await _context.ParkingGates
                .Where(g => g.GateType == "Entry")
                .Include(g => g.Operations)
                .ToListAsync();
        }

        public async Task<IEnumerable<ParkingGate>> GetAllExitGatesAsync()
        {
            return await _context.ParkingGates
                .Where(g => g.GateType == "Exit")
                .Include(g => g.Operations)
                .ToListAsync();
        }

        public async Task<ParkingGate> GetEntryGateByIdAsync(Guid id)
        {
            return await _context.ParkingGates
                .Where(g => g.GateType == "Entry")
                .Include(g => g.Operations)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<ParkingGate> GetExitGateByIdAsync(Guid id)
        {
            return await _context.ParkingGates
                .Where(g => g.GateType == "Exit")
                .Include(g => g.Operations)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<ParkingGate> GetGateByIdAsync(Guid gateId)
        {
            return await _context.ParkingGates
                .Include(g => g.Operations)
                .FirstOrDefaultAsync(g => g.Id == gateId);
        }

        public async Task<bool> IsGateOperationalAsync(Guid gateId)
        {
            var gate = await GetGateByIdAsync(gateId);
            return gate != null && gate.IsOnline;
        }

        public async Task<GateStatus> GetGateStatusAsync(Guid gateId)
        {
            var gate = await GetGateByIdAsync(gateId);
            
            return new GateStatus
            {
                IsOperational = gate?.IsOnline ?? false,
                StatusDescription = gate?.Status ?? "Unknown",
                LastChecked = DateTime.UtcNow
            };
        }

        public async Task<ParkingGate> OpenGateAsync(Guid gateId)
        {
            var gate = await GetGateByIdAsync(gateId);
            if (gate == null)
                throw new ArgumentException("Gate not found", nameof(gateId));

            gate.IsOnline = true;
            gate.Status = "Open";
            
            _context.ParkingGates.Update(gate);
            await _context.SaveChangesAsync();

            return gate;
        }

        public async Task<ParkingGate> CloseGateAsync(Guid gateId)
        {
            var gate = await GetGateByIdAsync(gateId);
            if (gate == null)
                throw new ArgumentException("Gate not found", nameof(gateId));

            gate.IsOnline = false;
            gate.Status = "Closed";
            
            _context.ParkingGates.Update(gate);
            await _context.SaveChangesAsync();

            return gate;
        }
    }
}
