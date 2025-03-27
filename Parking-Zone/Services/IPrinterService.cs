using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IPrinterService
    {
        Task<bool> InitializePrinterAsync(PrinterConfiguration config);
        Task<bool> PrintTicketAsync(string gateId, ParkingTicket ticket);
        Task<bool> PrintReceiptAsync(string gateId, ParkingTransaction transaction);
        Task<bool> IsOperationalAsync(string gateId);
        Task<bool> DisconnectAsync(string gateId);
        Task<Printer?> GetPrinterByGateIdAsync(string gateId);
        Task<IEnumerable<Printer>> GetAllPrintersAsync();
        Task<bool> UpdatePrinterConfigAsync(string gateId, PrinterConfig config);
        Task<PrinterConfig?> GetPrinterConfigAsync(string gateId);
    }
} 