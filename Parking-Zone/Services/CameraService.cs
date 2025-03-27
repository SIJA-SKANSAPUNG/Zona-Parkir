using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Parking_Zone.Hardware;
using Parking_Zone.Models;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Services
{
    public class CameraService : ICameraService
    {
        private readonly ILogger<CameraService> _logger;
        private readonly IHardwareManager _hardwareManager;
        private readonly IIPCameraService _ipCameraService;

        public CameraService(
            ILogger<CameraService> logger,
            IHardwareManager hardwareManager,
            IIPCameraService ipCameraService)
        {
            _logger = logger;
            _hardwareManager = hardwareManager;
            _ipCameraService = ipCameraService;
        }

        public async Task<bool> InitializeCameraAsync(Parking_Zone.Models.CameraConfiguration config)
        {
            _logger.LogInformation("Basic implementation of InitializeCameraAsync");
            return true;
        }

        public async Task<bool> InitializeServiceCameraAsync(Services.Models.CameraConfiguration config)
        {
            try
            {
                _logger.LogInformation($"Initializing camera {config.IpAddress}");
                var deviceConfig = new DeviceConfiguration
                {
                    DeviceId = config.CameraId,
                    DeviceType = "Camera",
                    IpAddress = config.IpAddress,
                    Port = config.Port,
                    Settings = new
                    {
                        Username = config.Username,
                        Password = config.Password,
                        StreamUrl = config.StreamUrl,
                        SnapshotUrl = config.SnapshotUrl,
                        ImageQuality = config.ImageQuality,
                        ImageResolutionWidth = config.ImageResolutionWidth,
                        ImageResolutionHeight = config.ImageResolutionHeight
                    }
                };

                var initialized = await _hardwareManager.InitializeDeviceAsync(deviceConfig);
                if (!initialized)
                {
                    return false;
                }

                // For IP cameras, check if they're online
                if (!string.IsNullOrEmpty(config.IpAddress))
                {
                    return await _ipCameraService.IsOnlineAsync(config.IpAddress);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error initializing camera {config.IpAddress}");
                return false;
            }
        }

        public async Task<byte[]?> CaptureImageAsync(string gateId, string reason)
        {
            try
            {
                var camera = await GetCameraByGateIdAsync(gateId);
                if (camera == null || !camera.IsOperational)
                {
                    throw new InvalidOperationException($"Camera at gate {gateId} is not available");
                }

                _logger.LogInformation($"Capturing image at gate {gateId} for reason: {reason}");

                // For IP cameras, use the IPCameraService
                if (!string.IsNullOrEmpty(camera.IpAddress))
                {
                    var base64Image = await _ipCameraService.CaptureImageAsync(camera.IpAddress, camera.Port);
                    if (base64Image.StartsWith("data:image/jpeg;base64,"))
                    {
                        return Convert.FromBase64String(base64Image.Substring("data:image/jpeg;base64,".Length));
                    }
                }

                // For other cameras, use the hardware manager
                var result = await _hardwareManager.SendCommandAsync(gateId, "CAPTURE", new { Reason = reason });
                if (!result)
                {
                    throw new InvalidOperationException($"Failed to capture image at gate {gateId}");
                }

                // Get the image data from the hardware manager
                var response = await _hardwareManager.GetDeviceSettingsAsync(gateId);
                if (response is byte[] imageData)
                {
                    return imageData;
                }

                throw new InvalidOperationException($"Failed to get image data from gate {gateId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error capturing image at gate {gateId}");
                return null;
            }
        }

        public async Task<bool> IsOperationalAsync(string gateId)
        {
            try
            {
                return await _hardwareManager.IsDeviceOperationalAsync(gateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking camera operational status at gate {gateId}");
                return false;
            }
        }

        public async Task<bool> DisconnectAsync(string gateId)
        {
            try
            {
                return await _hardwareManager.DisconnectDeviceAsync(gateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error disconnecting camera at gate {gateId}");
                return false;
            }
        }

        public async Task<Camera?> GetCameraByGateIdAsync(string gateId)
        {
            try
            {
                var config = await _hardwareManager.GetDeviceConfigurationAsync(gateId);
                if (config == null) return null;

                return new Camera
                {
                    GateId = gateId,
                    IsOperational = await IsOperationalAsync(gateId),
                    Settings = await GetCameraSettingsAsync(gateId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving camera information for gate {gateId}");
                return null;
            }
        }

        public async Task<IEnumerable<Camera>> GetAllCamerasAsync()
        {
            try
            {
                var cameras = new List<Camera>();
                var configs = await _hardwareManager.GetAllDeviceConfigurationsAsync();

                foreach (var config in configs)
                {
                    if (config.DeviceType == "Camera")
                    {
                        var camera = await GetCameraByGateIdAsync(config.DeviceId);
                        if (camera != null)
                        {
                            cameras.Add(camera);
                        }
                    }
                }

                return cameras;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all cameras");
                return new List<Camera>();
            }
        }

        public async Task<bool> UpdateCameraSettingsAsync(string gateId, CameraSettings settings)
        {
            try
            {
                return await _hardwareManager.UpdateDeviceSettingsAsync(gateId, settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating camera settings for gate {gateId}");
                return false;
            }
        }

        public async Task<CameraSettings?> GetCameraSettingsAsync(string gateId)
        {
            try
            {
                return await _hardwareManager.GetDeviceSettingsAsync(gateId) as CameraSettings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving camera settings for gate {gateId}");
                return null;
            }
        }
    }
} 