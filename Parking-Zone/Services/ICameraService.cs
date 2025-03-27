using System.Threading.Tasks;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface ICameraService
    {
        Task<bool> InitializeCameraAsync(CameraConfiguration config);
        Task<byte[]?> CaptureImageAsync(string gateId, string reason);
        Task<bool> IsOperationalAsync(string gateId);
        Task<bool> DisconnectAsync(string gateId);
        Task<Camera?> GetCameraByGateIdAsync(string gateId);
        Task<IEnumerable<Camera>> GetAllCamerasAsync();
        Task<bool> UpdateCameraSettingsAsync(string gateId, CameraSettings settings);
        Task<CameraSettings?> GetCameraSettingsAsync(string gateId);
    }
}
 