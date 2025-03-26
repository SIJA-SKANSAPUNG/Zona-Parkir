using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> RecordEntry(string plateNumber, string vehicleType, byte[] photoEntry, Guid parkingZoneId);
        Task<Vehicle> RecordExit(Guid vehicleId, byte[] photoExit);
        Task<Vehicle> GetVehicleById(Guid id);
        Task<Vehicle> GetVehicleByPlateNumber(string plateNumber);
        Task<IEnumerable<Vehicle>> GetAllVehicles();
        Task<IEnumerable<Vehicle>> GetVehiclesInsideParking();
        Task<bool> IsVehicleInside(string plateNumber);
        Task<string> GenerateTicketBarcode(Vehicle vehicle);
    }
} 