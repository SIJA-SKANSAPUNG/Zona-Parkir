using System.Collections.Generic;
using System.Linq;

namespace Parking_Zone.Extensions
{
    public static class ModelViewModelExtensions
    {
        public static List<Parking_Zone.ViewModels.VehicleEntry> ToViewModelList(
            this List<Parking_Zone.Models.VehicleEntry> modelList)
        {
            return modelList.Select(m => new Parking_Zone.ViewModels.VehicleEntry
            {
                LicensePlate = m.LicensePlate,
                VehicleType = m.VehicleType,
                EntryTime = m.EntryTime,
                GateId = m.GateId,
                VehicleId = m.VehicleId
            }).ToList();
        }

        public static string GetVehicleTypeString(this Parking_Zone.Models.VehicleType vehicleType)
        {
            return vehicleType switch
            {
                Parking_Zone.Models.VehicleType.Car => "Car",
                Parking_Zone.Models.VehicleType.Motorcycle => "Motorcycle",
                Parking_Zone.Models.VehicleType.Truck => "Truck",
                Parking_Zone.Models.VehicleType.Bus => "Bus",
                Parking_Zone.Models.VehicleType.Bicycle => "Bicycle",
                _ => "Unknown"
            };
        }
    }
}
