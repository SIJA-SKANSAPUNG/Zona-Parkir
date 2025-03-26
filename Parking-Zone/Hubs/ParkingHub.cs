using Microsoft.AspNetCore.SignalR;
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

        public override async Task OnConnectedAsync()
        {
            // Optional: Log connection or perform any setup
            await base.OnConnectedAsync();
        }

        public async Task SendGateStatus(string gateId, bool isOpen)
        {
            await Clients.All.SendAsync("ReceiveGateStatus", gateId, isOpen);
        }

        public async Task SendVehicleEntry(string plateNumber)
        {
            await Clients.All.SendAsync("ReceiveVehicleEntry", plateNumber);
        }

        public async Task SendVehicleExit(string plateNumber)
        {
            await Clients.All.SendAsync("ReceiveVehicleExit", plateNumber);
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
