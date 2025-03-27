using Parking_Zone.Services;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class ParkingServiceExtensions
    {
        public static async Task<decimal> GetBaseRateAsync(
            this IParkingService service)
        {
            return await service.GetBaseRateAsync();
        }
    }
}
