using System;
using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services.Interfaces
{
    public interface IPrinterService
    {
        /// <summary>
        /// Prints an entry ticket for a parking transaction
        /// </summary>
        /// <param name="ticket">Parking ticket to print</param>
        /// <returns>Task representing the print operation with a boolean result</returns>
        Task<bool> PrintEntryTicket(ParkingTicket ticket);

        /// <summary>
        /// Prints an exit ticket for a parking transaction
        /// </summary>
        /// <param name="ticket">Parking ticket to print</param>
        /// <returns>Task representing the print operation</returns>
        Task PrintExitTicket(ParkingTicket ticket);

        /// <summary>
        /// Checks the status of the printer
        /// </summary>
        /// <param name="printerId">Unique identifier of the printer</param>
        /// <returns>Printer status</returns>
        Task<string> GetPrinterStatus(Guid printerId);
    }
}
