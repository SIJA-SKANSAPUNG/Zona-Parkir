using Microsoft.Extensions.Logging;
using Parking_Zone.Services.Interfaces;
using Parking_Zone.Hardware;
using System;
using System.IO;
using System.Threading.Tasks;
using Parking_Zone.Services.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parking_Zone.Services
{
    public class CameraService : ICameraService
    {
        private readonly ILogger<CameraService> _logger;
        private readonly IHardwareManager _hardwareManager;
        private readonly IIpCameraService _ipCameraService;

        public CameraService(
            ILogger<CameraService> logger,
            IHardwareManager hardwareManager,
            IIpCameraService ipCameraService)
        {
            _logger = logger;
            _hardwareManager = hardwareManager;
            _ipCameraService = ipCameraService;
        }

        public async Task<bool> InitializeCameraAsync(Parking_Zone.Services.Models.CameraConfiguration config)
        {
            try
            {
                if (config == null)
                {
                    _logger.LogError("Camera configuration is null");
                    return false;
                }

                // Validate camera configuration
                if (string.IsNullOrEmpty(config.CameraId))
                {
                    _logger.LogError("Camera ID is required");
                    return false;
                }

                // Send initialization command to hardware manager
                bool result = await _hardwareManager.SendCommandAsync(config.CameraId, "INITIALIZE", config);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error initializing camera {config?.CameraId}");
                return false;
            }
        }

        public async Task<bool> InitializeServiceCameraAsync(Parking_Zone.Services.Models.CameraConfiguration config)
        {
            try
            {
                // Additional service-specific initialization logic
                if (config == null)
                {
                    _logger.LogError("Camera configuration is null");
                    return false;
                }

                // Validate IP camera configuration
                if (!string.IsNullOrEmpty(config.IpAddress))
                {
                    // Attempt to connect to IP camera
                    bool ipCameraInitialized = await _ipCameraService.InitializeCameraAsync(config);
                    if (!ipCameraInitialized)
                    {
                        _logger.LogError($"Failed to initialize IP camera {config.CameraId}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error initializing service camera {config?.CameraId}");
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
                    Status = "Active",
                    LastSync = DateTime.UtcNow,
                    Name = config.Name ?? gateId
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving camera for gate {gateId}");
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
                    var camera = await GetCameraByGateIdAsync(config.DeviceId);
                    if (camera != null)
                    {
                        cameras.Add(camera);
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

        public async Task<bool> UpdateCameraSettingsAsync(string gateId, Parking_Zone.Services.Models.CameraSettings settings)
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

        public async Task<Parking_Zone.Services.Models.CameraSettings?> GetCameraSettingsAsync(string gateId)
        {
            try
            {
                return await _hardwareManager.GetDeviceSettingsAsync(gateId) as Parking_Zone.Services.Models.CameraSettings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving camera settings for gate {gateId}");
                return null;
            }
        }

        public async Task<bool> IsOperationalAsync(string gateId)
        {
            try
            {
                // Send a simple ping or status check command
                bool result = await _hardwareManager.SendCommandAsync(gateId, "STATUS", null);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking operational status for gate {gateId}");
                return false;
            }
        }

        public async Task<string> TakePhotoAsync(Parking_Zone.Services.Models.CameraConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (!config.IsActive)
            {
                throw new InvalidOperationException("Camera is not active");
            }

            try
            {
                string base64Image;
                
                // Attempt to capture image from IP camera
                if (!string.IsNullOrEmpty(config.IpAddress))
                {
                    base64Image = await _ipCameraService.CaptureImageAsync(config.IpAddress, config.Port);
                }
                else
                {
                    // Fallback to hardware manager capture
                    await _hardwareManager.SendCommandAsync(config.CameraId, "CAPTURE", null);
                    base64Image = await _hardwareManager.ReadResponseAsync();
                }

                // Generate unique filename
                string fileName = $"{Guid.NewGuid():N}.jpg";
                string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cameras");
                
                // Ensure directory exists
                Directory.CreateDirectory(directoryPath);
                
                string filePath = Path.Combine(directoryPath, fileName);

                // Convert base64 to byte array and save
                byte[] imageBytes = Convert.FromBase64String(base64Image.Split(',')[1]);
                await File.WriteAllBytesAsync(filePath, imageBytes);

                // Return relative path for web access
                return $"/images/cameras/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error taking photo");
                throw;
            }
        }

        public async Task<string> CaptureImageAsync(string gateId, string reason)
        {
            try
            {
                // Send capture command
                bool commandSent = await _hardwareManager.SendCommandAsync(gateId, "CAPTURE", new { Reason = reason });
                if (!commandSent)
                {
                    throw new ApplicationException($"Failed to send capture command to camera at gate {gateId}");
                }

                // Read response
                string imagePath = await _hardwareManager.ReadResponseAsync();
                if (string.IsNullOrEmpty(imagePath))
                {
                    throw new ApplicationException($"No response from camera at gate {gateId}");
                }

                return imagePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error capturing image at gate {gateId}");
                throw;
            }
        }
    }
}