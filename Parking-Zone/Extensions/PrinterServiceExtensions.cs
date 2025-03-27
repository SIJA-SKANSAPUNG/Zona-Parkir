using Parking_Zone.Services;
using Parking_Zone.Models;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class PrinterServiceExtensions
    {
        public static async Task PrintTicket(this IPrinterService printerService, 
            string ticketNumber, 
            string plateNumber, 
            DateTime entryTime, 
            int vehicleType)
        {
            await printerService.PrintTicketAsync(ticketNumber, plateNumber, entryTime, vehicleType);
        }

        public static async Task PrintEntryTicket(this IPrinterService printerService, ParkingTicket ticket)
        {
            await printerService.PrintTicketAsync(
                ticket.TicketNumber, 
                ticket.VehicleEntry.LicensePlate, 
                ticket.VehicleEntry.EntryTime, 
                ticket.VehicleEntry.VehicleType.Id
            );
        }

        public static async Task PrintExitReceipt(this IPrinterService printerService, ParkingTransaction transaction)
        {
            await printerService.PrintReceiptAsync(transaction);
        }
    }
}
