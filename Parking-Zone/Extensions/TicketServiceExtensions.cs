using Parking_Zone.Services;
using Parking_Zone.Models;
using System.Threading.Tasks;

namespace Parking_Zone.Extensions
{
    public static class TicketServiceExtensions
    {
        public static async Task<ParkingTicket> GenerateTicketAsync(this ITicketService ticketService, VehicleEntry vehicleEntry)
        {
            return await ticketService.CreateTicketAsync(
                vehicleEntry.VehicleId, 
                vehicleEntry.GateId, 
                vehicleEntry.EntryTime
            );
        }

        public static async Task<ParkingTicket> GenerateTicketAsync(
            this ITicketService service, 
            string plateNumber, 
            int vehicleTypeId, 
            string entryPhotoPath, 
            Guid? parkingGateId = null, 
            Guid? operatorId = null)
        {
            return await service.GenerateTicketAsync(
                new VehicleEntry 
                { 
                    LicensePlate = plateNumber, 
                    VehicleType = new VehicleType { Id = vehicleTypeId }, 
                    EntryPhotoPath = entryPhotoPath,
                    ParkingGateId = parkingGateId,
                    OperatorId = operatorId
                });
        }

        public static async Task<ParkingTicket> CreateTicketAsync(
            this ITicketService service, 
            string plateNumber, 
            int vehicleTypeId, 
            string entryPhotoPath, 
            Guid? parkingGateId = null, 
            Guid? operatorId = null)
        {
            var vehicleEntry = new VehicleEntry
            {
                LicensePlate = plateNumber,
                VehicleType = new VehicleType { Id = vehicleTypeId },
                EntryPhotoPath = entryPhotoPath,
                ParkingGateId = parkingGateId,
                OperatorId = operatorId
            };

            return await service.GenerateTicketAsync(vehicleEntry);
        }

        public static string GetLicensePlate(this VehicleEntry vehicleEntry)
        {
            return vehicleEntry.LicensePlate;
        }

        public static int GetVehicleTypeId(this VehicleEntry vehicleEntry)
        {
            return vehicleEntry.VehicleType?.Id ?? 0;
        }

        public static string GetEntryPhotoPath(this VehicleEntry vehicleEntry)
        {
            return vehicleEntry.EntryPhotoPath;
        }

        public static Guid? GetParkingGateId(this VehicleEntry vehicleEntry)
        {
            return vehicleEntry.ParkingGateId;
        }
    }
}
