using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public class StubCameraService : ICameraService
    {
        public Task<bool> InitializeCameraAsync(CameraConfiguration config)
        {
            return Task.FromResult(true);
        }

        public Task<byte[]?> CaptureImageAsync(string gateId, string reason)
        {
            return Task.FromResult<byte[]?>(new byte[0]);
        }

        public Task<bool> IsOperationalAsync(string gateId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DisconnectAsync(string gateId)
        {
            return Task.FromResult(true);
        }

        public Task<Camera?> GetCameraByGateIdAsync(string gateId)
        {
            return Task.FromResult<Camera?>(new Camera());
        }

        public Task<IEnumerable<Camera>> GetAllCamerasAsync()
        {
            return Task.FromResult<IEnumerable<Camera>>(new List<Camera>());
        }

        public Task<bool> UpdateCameraSettingsAsync(string gateId, CameraSettings settings)
        {
            return Task.FromResult(true);
        }

        public Task<CameraSettings?> GetCameraSettingsAsync(string gateId)
        {
            return Task.FromResult<CameraSettings?>(new CameraSettings());
        }

        // Add TakePhoto method
        public Task<byte[]> TakePhoto()
        {
            // Return a dummy byte array
            return Task.FromResult(new byte[1024]);
        }
    }
} 