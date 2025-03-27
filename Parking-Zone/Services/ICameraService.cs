using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Services
{
    public interface ICameraService
    {
        Task<bool> InitializeCameraAsync(CameraConfiguration config);
        Task<bool> InitializeServiceCameraAsync(CameraConfiguration config);
        Task<Camera?> GetCameraByGateIdAsync(string gateId);
        Task<IEnumerable<Camera>> GetAllCamerasAsync();
        Task<bool> UpdateCameraSettingsAsync(string gateId, CameraSettings settings);
        Task<CameraSettings?> GetCameraSettingsAsync(string gateId);
        Task<string> TakePhotoAsync(CameraConfiguration config);
        Task<bool> IsOperationalAsync(string gateId);
        Task<string> CaptureImageAsync(string gateId, string reason);
    }
}