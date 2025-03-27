using Microsoft.Extensions.Diagnostics.HealthChecks;
using Parking_Zone.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Parking_Zone.HealthChecks
{
    public class CameraHealthCheck : IHealthCheck
    {
        private readonly ICameraService _cameraService;

        public CameraHealthCheck(ICameraService cameraService)
        {
            _cameraService = cameraService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try 
            {
                var isHealthy = await _cameraService.CheckCameraStatusAsync();
                return isHealthy 
                    ? HealthCheckResult.Healthy("Camera is functioning correctly")
                    : HealthCheckResult.Unhealthy("Camera is not responding");
            }
            catch
            {
                return HealthCheckResult.Unhealthy("Camera health check failed");
            }
        }
    }
}
