using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Services
{
    public class StubCameraService : ICameraService
    {
        public async Task<bool> InitializeCameraAsync(CameraConfiguration config)
        {
            await Task.Delay(10);
            return true;
        }

        public async Task<bool> InitializeServiceCameraAsync(CameraConfiguration config)
        {
            await Task.Delay(10);
            return true;
        }

        public async Task<Camera?> GetCameraByGateIdAsync(string gateId)
        {
            await Task.Delay(10);
            return new Camera
            {
                GateId = gateId,
                IsOperational = true,
                Status = "Active",
                LastSync = DateTime.UtcNow,
                Name = $"Camera {gateId}",
                Model = "StubCamera",
                Resolution = "1920x1080"
            };
        }

        public async Task<IEnumerable<Camera>> GetAllCamerasAsync()
        {
            await Task.Delay(10);
            return new List<Camera>
            {
                new Camera { GateId = "GATE001", IsOperational = true, Status = "Active", Model = "StubCamera", Resolution = "1920x1080" },
                new Camera { GateId = "GATE002", IsOperational = true, Status = "Active", Model = "StubCamera", Resolution = "1920x1080" }
            };
        }

        public async Task<bool> UpdateCameraSettingsAsync(string gateId, CameraSettings settings)
        {
            await Task.Delay(10);
            return true;
        }

        public async Task<CameraSettings?> GetCameraSettingsAsync(string gateId)
        {
            await Task.Delay(10);
            return new CameraSettings
            {
                GateId = gateId,
                BaudRate = 9600,
                Resolution = "1920x1080"
            };
        }

        public async Task<string> TakePhotoAsync(CameraConfiguration config)
        {
            await Task.Delay(10);
            return $"/images/cameras/{config.CameraId}_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
        }

        public async Task<bool> IsOperationalAsync(string gateId)
        {
            await Task.Delay(10);
            return true;
        }

        public async Task<string> CaptureImageAsync(string gateId, string reason)
        {
            await Task.Delay(10);
            return $"/images/vehicles/{gateId}_{reason}_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
        }
    }
}