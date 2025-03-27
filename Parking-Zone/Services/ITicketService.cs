using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface ITicketService
    {
        Task<string> GenerateTicketNumberAsync();
        Task<bool> ValidateTicketAsync(string ticketNumber);
        Task<ParkingTransaction> GetTransactionByTicketAsync(string ticketNumber);
        Task<bool> VoidTicketAsync(string ticketNumber, string reason);
    }
} 