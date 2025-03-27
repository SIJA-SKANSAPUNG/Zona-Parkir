using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Models;
using Parking_Zone.Data;

namespace Parking_Zone.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly ILogger<VehicleService> _logger;
        private readonly ApplicationDbContext _context;

        public VehicleService(
            ILogger<VehicleService> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Vehicle> GetVehicleByLicensePlateAsync(string licensePlate)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .Include(v => v.VehicleType)
                    .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

                if (vehicle == null)
                {
                    _logger.LogWarning($"Vehicle with license plate {licensePlate} not found");
                    return null;
                }

                _logger.LogInformation($"Retrieved vehicle with license plate {licensePlate}");
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vehicle with license plate {licensePlate}");
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await _context.Vehicles
                    .Include(v => v.VehicleType)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {vehicles.Count} vehicles");
                return vehicles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all vehicles");
                throw;
            }
        }

        public async Task<Vehicle> RegisterVehicleAsync(string licensePlate, string vehicleTypeId)
        {
            try
            {
                var existingVehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

                if (existingVehicle != null)
                {
                    _logger.LogWarning($"Vehicle with license plate {licensePlate} already exists");
                    return existingVehicle;
                }

                var vehicleType = await _context.VehicleTypes
                    .FirstOrDefaultAsync(vt => vt.Id == vehicleTypeId);

                if (vehicleType == null)
                {
                    throw new KeyNotFoundException($"Vehicle type with ID {vehicleTypeId} not found");
                }

                var vehicle = new Vehicle
                {
                    LicensePlate = licensePlate,
                    VehicleTypeId = vehicleTypeId,
                    RegistrationDate = DateTime.UtcNow
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Registered new vehicle with license plate {licensePlate}");
                return vehicle;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering vehicle with license plate {licensePlate}");
                throw;
            }
        }

        public async Task<bool> UpdateVehicleTypeAsync(string licensePlate, string newVehicleTypeId)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

                if (vehicle == null)
                {
                    _logger.LogWarning($"Vehicle with license plate {licensePlate} not found");
                    return false;
                }

                var vehicleType = await _context.VehicleTypes
                    .FirstOrDefaultAsync(vt => vt.Id == newVehicleTypeId);

                if (vehicleType == null)
                {
                    _logger.LogWarning($"Vehicle type with ID {newVehicleTypeId} not found");
                    return false;
                }

                vehicle.VehicleTypeId = newVehicleTypeId;
                vehicle.LastUpdated = DateTime.UtcNow;

                _context.Vehicles.Update(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated vehicle type for license plate {licensePlate}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating vehicle type for license plate {licensePlate}");
                return false;
            }
        }

        public async Task<bool> DeregisterVehicleAsync(string licensePlate)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.LicensePlate == licensePlate);

                if (vehicle == null)
                {
                    _logger.LogWarning($"Vehicle with license plate {licensePlate} not found");
                    return false;
                }

                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deregistered vehicle with license plate {licensePlate}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deregistering vehicle with license plate {licensePlate}");
                return false;
            }
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