using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Hardware
{
    public interface IHardwareManager
    {
        Task<bool> InitializeDeviceAsync(PrinterConfiguration config);
        Task<bool> SendCommandAsync(string deviceId, string command, object data);
        Task<bool> IsDeviceOperationalAsync(string deviceId);
        Task<bool> DisconnectDeviceAsync(string deviceId);
        Task<DeviceConfiguration?> GetDeviceConfigurationAsync(string deviceId);
        Task<IEnumerable<DeviceConfiguration>> GetAllDeviceConfigurationsAsync();
        Task<bool> UpdateDeviceSettingsAsync(string deviceId, object settings);
        Task<object?> GetDeviceSettingsAsync(string deviceId);
    }
} 