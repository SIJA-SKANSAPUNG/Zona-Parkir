using Parking_Zone.Services;
using Parking_Zone.Models;
using Parking_Zone.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace Parking_Zone.Extensions
{
    public static class VehicleServiceExtensions
    {
        public static async Task<IQueryable<VehicleType>> GetVehicleTypesAsync(this ApplicationDbContext context)
        {
            return context.VehicleTypes.AsQueryable();
        }

        public static int GetVehicleTypeId(this Vehicle vehicle)
        {
            return vehicle.VehicleType?.Id ?? 0;
        }

        public static string GetRegistrationDate(this Vehicle vehicle)
        {
            return vehicle.CreatedAt.ToString("yyyy-MM-dd");
        }
    }
}
