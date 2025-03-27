using System.Threading.Tasks;
using Parking_Zone.Services.Models;

namespace Parking_Zone.Services.Interfaces
{
    public interface IIpCameraService
    {
        Task<bool> InitializeCameraAsync(CameraConfiguration config);
        Task<bool> IsOnlineAsync(string ipAddress);
        Task<string> CaptureImageAsync(string ipAddress, int port);
    }
}
