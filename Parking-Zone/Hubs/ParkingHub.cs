using Microsoft.AspNetCore.SignalR;
using Parking_Zone.Models;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Hubs
{
    public class ParkingHub : Hub
    {
        public async Task JoinParkingZone(string parkingZoneId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, parkingZoneId);
        }

        public async Task LeaveParkingZone(string parkingZoneId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, parkingZoneId);
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
    }
}
