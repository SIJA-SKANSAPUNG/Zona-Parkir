using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Parking_Zone.Hubs
{
    public class GateHub : Hub
    {
        // This hub will be used for real-time communication with parking gates
        // Methods can be added here to handle gate events like entry, exit, etc.
        
        public async Task NotifyGateEvent(string gateId, string eventType, string message)
        {
            await Clients.All.SendAsync("ReceiveGateEvent", gateId, eventType, message);
        }
        
        public async Task NotifyVehicleEntry(string gateId, string plateNumber, DateTime timestamp)
        {
            await Clients.All.SendAsync("VehicleEntry", gateId, plateNumber, timestamp);
        }
        
        public async Task NotifyVehicleExit(string gateId, string plateNumber, DateTime timestamp)
        {
            await Clients.All.SendAsync("VehicleExit", gateId, plateNumber, timestamp);
        }
        
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("GateConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("GateDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}