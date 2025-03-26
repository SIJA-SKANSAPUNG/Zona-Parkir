using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingGateService
    {
        Task<bool> OpenGateAsync(int gateId);
        Task<bool> CloseGateAsync(int gateId);
        Task<bool> IsGateOpenAsync(int gateId);
        Task<ParkingGate> GetGateStatusAsync(int gateId);
        Task<bool> ValidateEntryAsync(string vehiclePlateNumber, int gateId);
        Task<bool> ValidateExitAsync(string vehiclePlateNumber, int gateId);
        Task LogGateOperationAsync(int gateId, string operation, string userId);
    }
}
