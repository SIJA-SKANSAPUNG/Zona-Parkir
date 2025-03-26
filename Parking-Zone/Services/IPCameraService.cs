using System;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public interface IIPCameraService
    {
        Task<string> CaptureImageAsync(string cameraIp, int port = 80);
        Task<bool> IsOnlineAsync(string cameraIp, int port = 80);
        Task<string> GetSnapshotUrlAsync(string cameraIp, int port = 80);
        Task<string> GetStreamUrlAsync(string cameraIp, int port = 80);
    }

    public class IPCameraService : IIPCameraService
    {
        private readonly ILogger<IPCameraService> _logger;

        public IPCameraService(ILogger<IPCameraService> logger)
        {
            _logger = logger;
        }

        public async Task<string> CaptureImageAsync(string cameraIp, int port = 80)
        {
            try
            {
                var snapshotUrl = await GetSnapshotUrlAsync(cameraIp, port);
                using var client = new HttpClient();
                var imageBytes = await client.GetByteArrayAsync(snapshotUrl);
                var base64Image = Convert.ToBase64String(imageBytes);
                return $"data:image/jpeg;base64,{base64Image}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing image from camera {CameraIp}:{Port}", cameraIp, port);
                throw;
            }
        }

        public async Task<bool> IsOnlineAsync(string cameraIp, int port = 80)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var response = await client.GetAsync($"http://{cameraIp}:{port}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public Task<string> GetSnapshotUrlAsync(string cameraIp, int port = 80)
        {
            return Task.FromResult($"http://{cameraIp}:{port}/snapshot.jpg");
        }

        public Task<string> GetStreamUrlAsync(string cameraIp, int port = 80)
        {
            return Task.FromResult($"http://{cameraIp}:{port}/stream");
        }
    }
} 