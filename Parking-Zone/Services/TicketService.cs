using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Models;
using Parking_Zone.Data;
using QRCoder;
using System.Drawing;
using System.IO;

namespace Parking_Zone.Services
{
    public class TicketService : ITicketService
    {
        private readonly ILogger<TicketService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IParkingTransactionService _transactionService;

        public TicketService(
            ILogger<TicketService> logger,
            ApplicationDbContext context,
            IParkingTransactionService transactionService)
        {
            _logger = logger;
            _context = context;
            _transactionService = transactionService;
        }

        public async Task<ParkingTicket> GenerateTicketAsync(VehicleEntry entry)
        {
            try
            {
                var ticketNumber = await GenerateTicketNumberAsync();
                var ticket = new ParkingTicket
                {
                    Id = ticketNumber,
                    VehicleNumber = entry.VehicleNumber,
                    EntryTime = DateTime.UtcNow,
                    Status = "Active",
                    GateId = entry.GateId
                };

                _logger.LogInformation("Generated ticket: {TicketNumber} for vehicle: {VehicleNumber}", 
                    ticketNumber, entry.VehicleNumber);
                
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating ticket for vehicle: {VehicleNumber}", 
                    entry.VehicleNumber);
                throw;
            }
        }

        public async Task<ParkingTicket> GetTicketByIdAsync(string ticketId)
        {
            try
            {
                // TODO: Implement actual database lookup
                _logger.LogInformation("Looking up ticket: {TicketId}", ticketId);
                if (await ValidateTicketAsync(ticketId))
                {
                    return new ParkingTicket { Id = ticketId };
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket: {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<ParkingTicket> GetTicketByVehicleNumberAsync(string vehicleNumber)
        {
            try
            {
                // TODO: Implement actual database lookup
                _logger.LogInformation("Looking up ticket for vehicle: {VehicleNumber}", vehicleNumber);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ticket for vehicle: {VehicleNumber}", vehicleNumber);
                throw;
            }
        }

        public async Task<bool> ValidateTicketAsync(string ticketNumber)
        {
            try
            {
                var ticket = await _context.ParkingTickets
                    .Include(t => t.Transaction)
                    .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

                if (ticket == null)
                {
                    _logger.LogWarning($"Ticket {ticketNumber} not found");
                    return false;
                }

                if (ticket.IsVoided)
                {
                    _logger.LogWarning($"Ticket {ticketNumber} is voided");
                    return false;
                }

                if (ticket.Transaction.Status == "Completed")
                {
                    _logger.LogWarning($"Ticket {ticketNumber} is already used");
                    return false;
                }

                _logger.LogInformation($"Ticket {ticketNumber} is valid");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating ticket {ticketNumber}");
                return false;
            }
        }

        public async Task<bool> VoidTicketAsync(string ticketNumber, string reason)
        {
            try
            {
                var ticket = await _context.ParkingTickets
                    .Include(t => t.Transaction)
                    .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

                if (ticket == null)
                {
                    _logger.LogWarning($"Ticket {ticketNumber} not found");
                    return false;
                }

                if (ticket.IsVoided)
                {
                    _logger.LogWarning($"Ticket {ticketNumber} is already voided");
                    return false;
                }

                ticket.IsVoided = true;
                ticket.VoidReason = reason;
                ticket.VoidedAt = DateTime.UtcNow;

                _context.ParkingTickets.Update(ticket);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Voided ticket {ticketNumber} with reason: {reason}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error voiding ticket {ticketNumber}");
                return false;
            }
        }

        public async Task<IEnumerable<ParkingTicket>> GetTicketsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // TODO: Implement actual database query
                _logger.LogInformation("Retrieving tickets between {StartDate} and {EndDate}", 
                    startDate, endDate);
                return new List<ParkingTicket>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tickets for date range");
                throw;
            }
        }

        public async Task<bool> UpdateTicketStatusAsync(string ticketId, string status)
        {
            try
            {
                var ticket = await GetTicketByIdAsync(ticketId);
                if (ticket == null)
                    return false;

                ticket.Status = status;
                // TODO: Update ticket in database
                _logger.LogInformation("Updated ticket {TicketId} status to: {Status}", ticketId, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ticket status");
                return false;
            }
        }

        public async Task<byte[]> GenerateQRCodeAsync(string ticketNumber)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(ticketNumber, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(qrCodeData);
                using var bitmap = qrCode.GetGraphic(20);
                using var stream = new MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code for ticket {TicketNumber}", ticketNumber);
                throw;
            }
        }

        public async Task<string> GenerateTicketNumberAsync()
        {
            try
            {
                // Generate a unique ticket number
                // Format: YYYYMMDD-HHMMSS-XXXX (where XXXX is a random number)
                var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
                var random = new Random();
                var randomPart = random.Next(1000, 9999).ToString();
                var ticketNumber = $"{timestamp}-{randomPart}";

                _logger.LogInformation($"Generated ticket number: {ticketNumber}");
                return ticketNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating ticket number");
                throw;
            }
        }

        public async Task<ParkingTransaction> GetTransactionByTicketAsync(string ticketNumber)
        {
            try
            {
                var ticket = await _context.ParkingTickets
                    .Include(t => t.Transaction)
                    .ThenInclude(t => t.Vehicle)
                    .Include(t => t.Transaction)
                    .ThenInclude(t => t.ParkingZone)
                    .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

                if (ticket == null)
                {
                    throw new KeyNotFoundException($"Ticket {ticketNumber} not found");
                }

                return ticket.Transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving transaction for ticket {ticketNumber}");
                throw;
            }
        }
    }
} 