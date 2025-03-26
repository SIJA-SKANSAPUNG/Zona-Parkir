using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(ApplicationDbContext context, ILogger<VehicleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Vehicle> RecordEntry(string plateNumber, string vehicleType, byte[] photoEntry, Guid parkingZoneId)
        {
            try
            {
                // Check if vehicle is already inside
                if (await IsVehicleInside(plateNumber))
                {
                    throw new InvalidOperationException($"Vehicle with plate number {plateNumber} is already inside the parking.");
                }

                var vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    PlateNumber = plateNumber,
                    VehicleType = vehicleType,
                    EntryTime = DateTime.UtcNow,
                    PhotoEntry = photoEntry,
                    IsInside = true,
                    TicketBarcode = await GenerateTicketBarcode(null) // Pass null since vehicle is not yet created
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Vehicle entry recorded: {plateNumber}");
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording vehicle entry: {plateNumber}");
                throw;
            }
        }

        public async Task<Vehicle> RecordExit(Guid vehicleId, byte[] photoExit)
        {
            try
            {
                var vehicle = await _context.Vehicles.FindAsync(vehicleId);
                if (vehicle == null)
                {
                    throw new KeyNotFoundException($"Vehicle with ID {vehicleId} not found.");
                }

                if (!vehicle.IsInside)
                {
                    throw new InvalidOperationException($"Vehicle with ID {vehicleId} is not inside the parking.");
                }

                vehicle.ExitTime = DateTime.UtcNow;
                vehicle.PhotoExit = photoExit;
                vehicle.IsInside = false;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Vehicle exit recorded: {vehicle.PlateNumber}");
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording vehicle exit: {vehicleId}");
                throw;
            }
        }

        public async Task<Vehicle> GetVehicleById(Guid id)
        {
            return await _context.Vehicles
                .Include(v => v.ParkingTransactions)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vehicle> GetVehicleByPlateNumber(string plateNumber)
        {
            return await _context.Vehicles
                .Include(v => v.ParkingTransactions)
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehicles()
        {
            return await _context.Vehicles
                .Include(v => v.ParkingTransactions)
                .OrderByDescending(v => v.EntryTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesInsideParking()
        {
            return await _context.Vehicles
                .Include(v => v.ParkingTransactions)
                .Where(v => v.IsInside)
                .OrderByDescending(v => v.EntryTime)
                .ToListAsync();
        }

        public async Task<bool> IsVehicleInside(string plateNumber)
        {
            return await _context.Vehicles
                .AnyAsync(v => v.PlateNumber == plateNumber && v.IsInside);
        }

        public async Task<string> GenerateTicketBarcode(Vehicle vehicle)
        {
            // Generate a unique barcode using timestamp and random number
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random();
            var randomNum = random.Next(1000, 9999).ToString();
            
            return $"PKR{timestamp}{randomNum}";
        }
    }
} 