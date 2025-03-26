using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Parking_Zone.Hubs;
using Parking_Zone.Models;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public class ParkingNotificationService : IParkingNotificationService
    {
        private readonly IHubContext<ParkingHub> _hubContext;
        private readonly ILogger<ParkingNotificationService> _logger;

        public ParkingNotificationService(
            IHubContext<ParkingHub> hubContext,
            ILogger<ParkingNotificationService> logger)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyVehicleEntered(Guid parkingZoneId, Vehicle vehicle)
        {
            try
            {
                await _hubContext.Clients.Group(parkingZoneId.ToString())
                    .SendAsync(ParkingHubMethods.VehicleEntered, new
                    {
                        vehicle.Id,
                        vehicle.PlateNumber,
                        vehicle.VehicleType,
                        EntryTime = DateTime.UtcNow
                    });

                _logger.LogInformation("Notified vehicle entry: {PlateNumber} in zone {ZoneId}", 
                    vehicle.PlateNumber, parkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying vehicle entry: {PlateNumber}", vehicle.PlateNumber);
            }
        }

        public async Task NotifyVehicleExited(Guid parkingZoneId, Vehicle vehicle)
        {
            try
            {
                await _hubContext.Clients.Group(parkingZoneId.ToString())
                    .SendAsync(ParkingHubMethods.VehicleExited, new
                    {
                        vehicle.Id,
                        vehicle.PlateNumber,
                        vehicle.VehicleType,
                        ExitTime = DateTime.UtcNow
                    });

                _logger.LogInformation("Notified vehicle exit: {PlateNumber} from zone {ZoneId}", 
                    vehicle.PlateNumber, parkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying vehicle exit: {PlateNumber}", vehicle.PlateNumber);
            }
        }

        public async Task NotifyGateStatusChanged(Guid parkingZoneId, ParkingGate gate)
        {
            try
            {
                await _hubContext.Clients.Group(parkingZoneId.ToString())
                    .SendAsync(ParkingHubMethods.GateStatusChanged, new
                    {
                        gate.Id,
                        gate.Name,
                        gate.IsOpen,
                        gate.IsOnline,
                        gate.LastActivity
                    });

                _logger.LogInformation("Notified gate status change: {GateId} in zone {ZoneId}", 
                    gate.Id, parkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying gate status change: {GateId}", gate.Id);
            }
        }

        public async Task NotifySpotStatusChanged(Guid parkingZoneId, ParkingSpot spot)
        {
            try
            {
                await _hubContext.Clients.Group(parkingZoneId.ToString())
                    .SendAsync(ParkingHubMethods.SpotStatusChanged, new
                    {
                        spot.Id,
                        spot.Number,
                        spot.IsOccupied,
                        spot.VehicleType,
                        UpdatedAt = DateTime.UtcNow
                    });

                _logger.LogInformation("Notified spot status change: {SpotId} in zone {ZoneId}", 
                    spot.Id, parkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying spot status change: {SpotId}", spot.Id);
            }
        }

        public async Task NotifyTransactionUpdated(Guid parkingZoneId, ParkingTransaction transaction)
        {
            try
            {
                await _hubContext.Clients.Group(parkingZoneId.ToString())
                    .SendAsync(ParkingHubMethods.TransactionUpdated, new
                    {
                        transaction.Id,
                        transaction.VehicleId,
                        transaction.Status,
                        transaction.StartTime,
                        transaction.EndTime,
                        transaction.Amount,
                        UpdatedAt = DateTime.UtcNow
                    });

                _logger.LogInformation("Notified transaction update: {TransactionId} in zone {ZoneId}", 
                    transaction.Id, parkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying transaction update: {TransactionId}", transaction.Id);
            }
        }

        public async Task NotifyOccupancyUpdated(Guid parkingZoneId, int totalSpots, int occupiedSpots)
        {
            try
            {
                var occupancyRate = totalSpots > 0 ? (double)occupiedSpots / totalSpots * 100 : 0;
                await _hubContext.Clients.Group(parkingZoneId.ToString())
                    .SendAsync(ParkingHubMethods.OccupancyUpdated, new
                    {
                        ParkingZoneId = parkingZoneId,
                        TotalSpots = totalSpots,
                        OccupiedSpots = occupiedSpots,
                        OccupancyRate = Math.Round(occupancyRate, 2),
                        UpdatedAt = DateTime.UtcNow
                    });

                _logger.LogInformation("Notified occupancy update: {OccupiedSpots}/{TotalSpots} in zone {ZoneId}", 
                    occupiedSpots, totalSpots, parkingZoneId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying occupancy update for zone {ZoneId}", parkingZoneId);
            }
        }
    }
}
