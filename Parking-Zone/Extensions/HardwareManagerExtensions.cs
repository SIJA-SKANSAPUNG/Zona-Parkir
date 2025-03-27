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
            var responseTask = hardwareManager.ReadResponseAsync(deviceId, command);
            return await responseTask;
        }
    }
}
