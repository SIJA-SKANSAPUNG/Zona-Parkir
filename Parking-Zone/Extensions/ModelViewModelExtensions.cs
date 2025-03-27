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
    }
}
