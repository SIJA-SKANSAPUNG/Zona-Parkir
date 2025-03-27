using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public class StubPrinterService : IPrinterService
    {
        public Task<bool> InitializePrinterAsync(PrinterConfiguration config)
        {
            return Task.FromResult(true);
        }

        public Task<bool> PrintTicketAsync(string gateId, ParkingTicket ticket)
        {
            return Task.FromResult(true);
        }

        public Task<bool> PrintReceiptAsync(string gateId, ParkingTransaction transaction)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsOperationalAsync(string gateId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DisconnectAsync(string gateId)
        {
            return Task.FromResult(true);
        }

        public Task<Printer?> GetPrinterByGateIdAsync(string gateId)
        {
            return Task.FromResult<Printer?>(new Printer());
        }

        public Task<IEnumerable<Printer>> GetAllPrintersAsync()
        {
            return Task.FromResult<IEnumerable<Printer>>(new List<Printer>());
        }

        public Task<bool> UpdatePrinterConfigAsync(string gateId, PrinterConfig config)
        {
            return Task.FromResult(true);
        }

        public Task<PrinterConfig?> GetPrinterConfigAsync(string gateId)
        {
            return Task.FromResult<PrinterConfig?>(new PrinterConfig());
        }
    }
} 