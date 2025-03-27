using Parking_Zone.Hardware;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class HardwareManagerExtensions
    {
        public static async Task<string> ReadResponseAsync(
            this IHardwareManager hardwareManager, 
            string deviceId, 
            byte[] command)
        {
            var responses = hardwareManager.ReadResponseAsync(deviceId, command);
            await foreach (var response in responses)
            {
                return response;
            }
            return string.Empty;
        }
    }
}
