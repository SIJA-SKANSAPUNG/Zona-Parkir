using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Parking_Zone.Services
{
    public class ConnectionStatusService
    {
        private readonly ILogger<ConnectionStatusService> _logger;
        private readonly ConcurrentDictionary<string, bool> _deviceStatus;

        public ConnectionStatusService(ILogger<ConnectionStatusService> logger)
        {
            _logger = logger;
            _deviceStatus = new ConcurrentDictionary<string, bool>();
        }

        public void UpdateDeviceStatus(string deviceId, bool isConnected)
        {
            _deviceStatus.AddOrUpdate(deviceId, isConnected, (key, oldValue) => isConnected);
            _logger.LogInformation($"Device {deviceId} connection status updated to: {isConnected}");
        }

        public bool IsDeviceConnected(string deviceId)
        {
            return _deviceStatus.TryGetValue(deviceId, out bool status) && status;
        }

        public void RemoveDevice(string deviceId)
        {
            if (_deviceStatus.TryRemove(deviceId, out _))
            {
                _logger.LogInformation($"Device {deviceId} removed from connection tracking");
            }
        }
    }
} 