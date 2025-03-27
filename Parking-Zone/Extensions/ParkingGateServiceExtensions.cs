using Parking_Zone.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class ParkingGateServiceExtensions
    {
        public static async Task<List<object>> GetAllGatesStatusAsync(
            this IParkingGateService service)
        {
            return await service.GetAllGatesStatusAsync();
        }
    }
}
