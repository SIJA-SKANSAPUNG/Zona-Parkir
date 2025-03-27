using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public interface IScannerService
    {
        Task<string> ScanBarcodeAsync();
        Task<bool> IsReadyAsync();
        Task<string> GetDeviceInfoAsync();
    }
} 