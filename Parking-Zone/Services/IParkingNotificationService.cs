using Parking_Zone.Models;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Services
{
    public interface IParkingNotificationService
    {
        Task NotifyVehicleEntered(Guid parkingZoneId, Vehicle vehicle);
        Task NotifyVehicleExited(Guid parkingZoneId, Vehicle vehicle);
        Task NotifyGateStatusChanged(Guid parkingZoneId, ParkingGate gate);
        Task NotifySpotStatusChanged(Guid parkingZoneId, ParkingSpot spot);
        Task NotifyTransactionUpdated(Guid parkingZoneId, ParkingTransaction transaction);
        Task NotifyOccupancyUpdated(Guid parkingZoneId, int totalSpots, int occupiedSpots);
    }
}
