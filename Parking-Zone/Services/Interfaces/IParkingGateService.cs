using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IParkingGateService
    {
        Task<IEnumerable<ParkingGate>> GetAllEntryGatesAsync();
        Task<IEnumerable<ParkingGate>> GetAllExitGatesAsync();
        Task<ParkingGate> GetEntryGateByIdAsync(Guid id);
        Task<ParkingGate> GetExitGateByIdAsync(Guid id);
        Task<bool> IsGateOperationalAsync(Guid gateId);
        Task<GateStatus> GetGateStatusAsync(Guid gateId);

        // Additional methods for GateController
        Task<ParkingGate> GetGateByIdAsync(Guid gateId);
        Task<ParkingGate> OpenGateAsync(Guid gateId);
        Task<ParkingGate> CloseGateAsync(Guid gateId);
    }

    public class GateStatus
    {
        public bool IsOperational { get; set; }
        public string StatusDescription { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
