using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IConnectionStatusService
    {
        Task<ConnectionStatus> GetStatusAsync();
        Task<bool> UpdateStatusAsync(string deviceId, string status);
        Task<bool> CheckDeviceConnectionAsync(string deviceId);
        Task<IEnumerable<ConnectionStatus>> GetAllDeviceStatusesAsync();
        Task<bool> ResetDeviceConnectionAsync(string deviceId);
    }

    public class ConnectionStatus
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
        public string Status { get; set; } = "Offline";
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;
        public string? LastError { get; set; }
    }
} 