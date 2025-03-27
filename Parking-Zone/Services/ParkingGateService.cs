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

        public async Task<bool> IsGateOperationalAsync(Guid gateId)
        {
            var gate = await _context.ParkingGates.FindAsync(gateId);
            return gate?.IsOperational ?? false;
        }

        public async Task<GateStatus> GetGateStatusAsync(Guid gateId)
        {
            var gate = await _context.ParkingGates.FindAsync(gateId);

            if (gate == null)
            {
                return new GateStatus
                {
                    IsOperational = false,
                    StatusDescription = "Gate not found",
                    LastChecked = DateTime.UtcNow
                };
            }

            return new GateStatus
            {
                IsOperational = gate.IsOperational,
                StatusDescription = gate.Status,
                LastChecked = gate.LastActivity ?? DateTime.UtcNow
            };
        }

        public async Task<ParkingGate> GetGateByIdAsync(Guid gateId)
        {
            var gate = await _context.ParkingGates
                .Include(g => g.Camera)
                .Include(g => g.Printer)
                .Include(g => g.Scanner)
                .FirstOrDefaultAsync(g => g.Id == gateId);

            if (gate == null)
                throw new KeyNotFoundException($"Gate with ID {gateId} not found");

            return gate;
        }

        public async Task<ParkingGate> OpenGateAsync(Guid gateId)
        {
            var gate = await GetGateByIdAsync(gateId);
            
            if (!gate.IsOperational)
            {
                throw new InvalidOperationException($"Gate {gateId} is not operational");
            }

            gate.IsOpen = true;
            gate.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return gate;
        }

        public async Task<ParkingGate> CloseGateAsync(Guid gateId)
        {
            var gate = await GetGateByIdAsync(gateId);

            gate.IsOpen = false;
            gate.LastActivity = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return gate;
        }
    }
}
