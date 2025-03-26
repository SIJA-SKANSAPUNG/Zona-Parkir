using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parking_Zone.Data;
using Parking_Zone.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Parking_Zone.Hubs
{
    public class ParkingHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ParkingHub> _logger;
        private readonly IPrinterService _printerService;
        private readonly IVehicleService _vehicleService;

        public ParkingHub(
            ApplicationDbContext context,
            ILogger<ParkingHub> logger,
            IPrinterService printerService,
            IVehicleService vehicleService)
        {
            _context = context;
            _logger = logger;
            _printerService = printerService;
            _vehicleService = vehicleService;
        }

        public async Task JoinParkingZone(string parkingZoneId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, parkingZoneId);
        }

        public async Task LeaveParkingZone(string parkingZoneId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, parkingZoneId);
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public async Task SendGateEvent(string gateId, string eventType, object data)
        {
            await Clients.All.SendAsync("ReceiveGateEvent", new
            {
                EventType = eventType,
                GateId = gateId,
                Timestamp = DateTime.UtcNow,
                Data = data
            });
        }

        public async Task NotifyGateStatus(string gateId, string gateState, string sensorState)
        {
            await Clients.All.SendAsync("GateStatusUpdated", new
            {
                GateId = gateId,
                Status = new
                {
                    Gate = gateState,
                    Sensor = sensorState
                },
                LastUpdated = DateTime.UtcNow
            });
        }

        public async Task NotifyVehicleDetected(string gateId)
        {
            await Clients.All.SendAsync("VehicleDetected", new
            {
                GateId = gateId,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyCommandResult(string gateId, string command, bool success, string message)
        {
            await Clients.All.SendAsync("CommandResult", new
            {
                GateId = gateId,
                Command = command,
                Success = success,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }

        public async Task NotifyPlateDetection(string licensePlate, bool isSuccessful)
        {
            await Clients.All.SendAsync("PlateDetectionResult", new { 
                licensePlate, 
                isSuccessful 
            });
        }

        public async Task NotifyBarrierStatus(bool isEntry, bool isOpen)
        {
            await Clients.All.SendAsync("BarrierStatusChanged", new { 
                isEntry, 
                isOpen,
                barrierType = isEntry ? "Entry" : "Exit",
                status = isOpen ? "Open" : "Closed"
            });
        }

        public async Task EnterVehicle(string plateNumber, string vehicleType)
        {
            try
            {
                var parkingGate = await _context.ParkingGates.FirstOrDefaultAsync();
                if (parkingGate == null)
                {
                    await Clients.Caller.SendAsync("ShowError", "Entry gate not configured");
                    return;
                }

                var vehicle = await _vehicleService.RecordEntry(plateNumber, vehicleType, null, parkingGate.ParkingZoneId);
                if (vehicle != null)
                {
                    var transaction = await _context.ParkingTransactions
                        .Where(t => t.VehicleId == vehicle.Id)
                        .OrderByDescending(t => t.EntryTime)
                        .FirstOrDefaultAsync();
                    if (transaction != null)
                    {
                        await _printerService.PrintEntryTicket(transaction);
                        await Clients.Caller.SendAsync("ShowSuccess", "Vehicle entered successfully");
                        await SendVehicleEntry(plateNumber);
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("ShowError", "Failed to record vehicle entry");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle entry");
                await Clients.Caller.SendAsync("ShowError", $"Error: {ex.Message}");
            }
        }

        public async Task ExitVehicle(string plateNumber)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber && v.IsInside);
                if (vehicle == null)
                {
                    await Clients.Caller.SendAsync("ShowError", "Vehicle not found or already exited");
                    return;
                }

                var updatedVehicle = await _vehicleService.RecordExit(vehicle.Id, null);
                if (updatedVehicle != null)
                {
                    var transaction = await _context.ParkingTransactions
                        .Where(t => t.VehicleId == vehicle.Id && t.ExitTime != null)
                        .OrderByDescending(t => t.ExitTime)
                        .FirstOrDefaultAsync();
                    if (transaction != null)
                    {
                        await _printerService.PrintExitReceipt(transaction);
                        await Clients.Caller.SendAsync("ShowSuccess", "Vehicle exited successfully");
                        await SendVehicleExit(plateNumber);
                    }
                }
                else
                {
                    await Clients.Caller.SendAsync("ShowError", "Failed to record vehicle exit");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle exit");
                await Clients.Caller.SendAsync("ShowError", $"Error: {ex.Message}");
            }
        }

        public async Task PrintReceipt(object receiptData)
        {
            await Clients.All.SendAsync("PrintReceipt", receiptData);
        }
    }

    public static class ParkingHubMethods
    {
        public const string VehicleEntered = "VehicleEntered";
        public const string VehicleExited = "VehicleExited";
        public const string GateStatusChanged = "GateStatusChanged";
        public const string SpotStatusChanged = "SpotStatusChanged";
        public const string TransactionUpdated = "TransactionUpdated";
        public const string OccupancyUpdated = "OccupancyUpdated";
        public const string PlateDetected = "PlateDetected";
        public const string BarrierStateChanged = "BarrierStateChanged";
        public const string GateEventReceived = "GateEventReceived";
        public const string CommandResultReceived = "CommandResultReceived";
    }
}
