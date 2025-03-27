using Parking_Zone.Services;
using Parking_Zone.Models;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class PrinterServiceExtensions
    {
        public static async Task<bool> PrintTicket(this IPrinterService printerService, ParkingTicket ticket)
        {
            if (ticket == null) return false;

            return await printerService.PrintTicketAsync(
                ticket.TicketNumber, 
                ticket.Vehicle?.LicensePlate ?? ticket.VehicleLicensePlate, 
                ticket.IssueTime ?? DateTime.Now, 
                ticket.Vehicle?.VehicleType != null ? int.Parse(ticket.Vehicle.VehicleType) : 0
            );
        }

        public static async Task PrintExitReceipt(this IPrinterService printerService, ParkingTransaction transaction)
        {
            await printerService.PrintReceiptAsync(transaction.TicketNumber, transaction);
        }
    }
}
