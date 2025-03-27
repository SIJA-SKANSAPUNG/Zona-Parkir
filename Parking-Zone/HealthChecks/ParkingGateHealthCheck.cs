using Microsoft.Extensions.Diagnostics.HealthChecks;
using Parking_Zone.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Parking_Zone.HealthChecks
{
    public class ParkingGateHealthCheck : IHealthCheck
    {
        private readonly IParkingGateService _parkingGateService;

        public ParkingGateHealthCheck(IParkingGateService parkingGateService)
        {
            _parkingGateService = parkingGateService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var gatesStatus = await _parkingGateService.GetAllGatesStatusAsync();
                
                if (gatesStatus.AllGatesOperational)
                {
                    return HealthCheckResult.Healthy("All parking gates are operational.");
                }
                else
                {
                    return HealthCheckResult.Degraded($"Some parking gates are not operational. Details: {gatesStatus.StatusDescription}");
                }
            }
            catch
            {
                return HealthCheckResult.Unhealthy("Unable to check parking gate status.");
            }
        }
    }
}
