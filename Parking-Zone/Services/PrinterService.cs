using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Parking_Zone.Models;
using Parking_Zone.Hardware;
using Parking_Zone.Services.Models;
using HardwarePrinterConfig = Parking_Zone.Hardware.PrinterConfig;
using ModelsPrinterConfig = Parking_Zone.Models.PrinterConfig;

namespace Parking_Zone.Services
{
    public class PrinterService : IPrinterService
    {
        private readonly ILogger<PrinterService> _logger;
        private readonly IHardwareManager _hardwareManager;

        public PrinterService(ILogger<PrinterService> logger, IHardwareManager hardwareManager)
        {
            _logger = logger;
            _hardwareManager = hardwareManager;
        }

        public async Task<bool> InitializePrinterAsync(Parking_Zone.Models.PrinterConfiguration config)
        {
            _logger.LogInformation("Basic implementation of InitializePrinterAsync");
            return true;
        }

        public async Task<bool> InitializeServicePrinterAsync(Services.Models.PrinterConfiguration config)
        {
            try
            {
                _logger.LogInformation($"Initializing printer with configuration: {config.Name}");
                var deviceConfig = new DeviceConfiguration
                {
                    DeviceId = config.PrinterId,
                    DeviceType = "Printer",
                    Settings = new
                    {
                        Name = config.Name,
                        PortName = config.PortName,
                        BaudRate = config.BaudRate,
                        DataBits = config.DataBits,
                        Parity = config.Parity,
                        StopBits = config.StopBits,
                        Handshake = config.Handshake,
                        Model = config.Model,
                        Manufacturer = config.Manufacturer,
                        SerialNumber = config.SerialNumber,
                        PaperWidth = config.PaperWidth,
                        CharactersPerLine = config.CharactersPerLine,
                        AutoCut = config.AutoCut,
                        CutLength = config.CutLength,
                        HeaderText = config.HeaderText,
                        FooterText = config.FooterText,
                        PrintLogo = config.PrintLogo,
                        LogoPath = config.LogoPath
                    }
                };
                return await _hardwareManager.InitializeDeviceAsync(deviceConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing printer");
                return false;
            }
        }

        public async Task<bool> PrintTicketAsync(string gateId, ParkingTicket ticket)
        {
            try
            {
                var printer = await GetPrinterByGateIdAsync(gateId);
                if (printer == null || !printer.IsOperational)
                {
                    throw new ApplicationException($"Printer at gate {gateId} is not available");
                }

                // Format ticket data for printing
                var printData = FormatTicketData(ticket);
                
                // Send print command
                var success = await _hardwareManager.SendCommandAsync(gateId, "PRINT", printData);
                if (!success)
                {
                    throw new ApplicationException($"Failed to send print command to printer at gate {gateId}");
                }

                _logger.LogInformation($"Ticket printed successfully at gate {gateId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error printing ticket at gate {gateId}");
                return false;
            }
        }

        public async Task<bool> PrintReceiptAsync(string gateId, ParkingTransaction transaction)
        {
            try
            {
                var printer = await GetPrinterByGateIdAsync(gateId);
                if (printer == null || !printer.IsOperational)
                {
                    throw new ApplicationException($"Printer at gate {gateId} is not available");
                }

                // Format transaction data for printing
                var printData = FormatReceiptData(transaction);
                
                // Send print command
                var success = await _hardwareManager.SendCommandAsync(gateId, "PRINT_RECEIPT", printData);
                if (!success)
                {
                    throw new ApplicationException($"Failed to send print receipt command to printer at gate {gateId}");
                }

                _logger.LogInformation($"Receipt printed successfully at gate {gateId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error printing receipt at gate {gateId}");
                return false;
            }
        }

        public async Task<bool> IsOperationalAsync(string gateId)
        {
            try
            {
                return await _hardwareManager.IsDeviceOperationalAsync(gateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking printer status at gate {gateId}");
                return false;
            }
        }

        public async Task<bool> DisconnectAsync(string gateId)
        {
            try
            {
                return await _hardwareManager.DisconnectDeviceAsync(gateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error disconnecting printer at gate {gateId}");
                return false;
            }
        }

        public async Task<Printer?> GetPrinterByGateIdAsync(string gateId)
        {
            try
            {
                var config = await _hardwareManager.GetDeviceConfigurationAsync(gateId);
                if (config == null) return null;

                return new Printer
                {
                    GateId = gateId,
                    IsOperational = await IsOperationalAsync(gateId),
                    Config = await GetPrinterConfigAsync(gateId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving printer information for gate {gateId}");
                return null;
            }
        }

        public async Task<IEnumerable<Printer>> GetAllPrintersAsync()
        {
            try
            {
                var printers = new List<Printer>();
                var configs = await _hardwareManager.GetAllDeviceConfigurationsAsync();

                foreach (var config in configs)
                {
                    var printer = await GetPrinterByGateIdAsync(config.DeviceId);
                    if (printer != null)
                    {
                        printers.Add(printer);
                    }
                }

                return printers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all printers");
                return new List<Printer>();
            }
        }

        public async Task<bool> UpdatePrinterConfigAsync(string gateId, ModelsPrinterConfig config)
        {
            try
            {
                _logger.LogInformation($"Updating printer configuration for printer: {gateId}");
                return await _hardwareManager.UpdateDeviceSettingsAsync(gateId, config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating printer configuration for printer: {gateId}");
                return false;
            }
        }

        public async Task<ModelsPrinterConfig?> GetPrinterConfigAsync(string gateId)
        {
            try
            {
                _logger.LogInformation($"Getting printer configuration for printer: {gateId}");
                return await _hardwareManager.GetDeviceSettingsAsync(gateId) as ModelsPrinterConfig;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting printer configuration for printer: {gateId}");
                return null;
            }
        }

        private string FormatTicketData(ParkingTicket ticket)
        {
            return $@"
PARKING TICKET
--------------
Ticket ID: {ticket.Id}
Vehicle: {ticket.VehicleNumber}
Entry Time: {ticket.EntryTime:yyyy-MM-dd HH:mm:ss}
Gate: {ticket.GateId}
--------------
PLEASE KEEP THIS TICKET SAFE
";
        }

        private string FormatReceiptData(ParkingTransaction transaction)
        {
            return $@"
PARKING RECEIPT
---------------
Ticket ID: {transaction.Id}
Vehicle: {transaction.VehicleNumber}
Entry Time: {transaction.EntryTime:yyyy-MM-dd HH:mm:ss}
Exit Time: {transaction.ExitTime:yyyy-MM-dd HH:mm:ss}
Duration: {(transaction.ExitTime - transaction.EntryTime):hh\:mm}
Total Charge: {transaction.Cost:C2}
Payment Method: {transaction.PaymentMethod}
Gate: {transaction.GateId}
---------------
THANK YOU FOR PARKING
";
        }
    }
}