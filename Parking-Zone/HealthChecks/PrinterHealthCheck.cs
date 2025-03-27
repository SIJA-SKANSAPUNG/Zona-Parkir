using Microsoft.Extensions.Diagnostics.HealthChecks;
using Parking_Zone.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Parking_Zone.HealthChecks
{
    public class PrinterHealthCheck : IHealthCheck
    {
        private readonly IPrinterService _printerService;

        public PrinterHealthCheck(IPrinterService printerService)
        {
            _printerService = printerService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try 
            {
                var isHealthy = await _printerService.CheckPrinterStatusAsync();
                return isHealthy 
                    ? HealthCheckResult.Healthy("Printer is functioning correctly")
                    : HealthCheckResult.Unhealthy("Printer is not responding");
            }
            catch
            {
                return HealthCheckResult.Unhealthy("Printer health check failed");
            }
        }
    }
}
