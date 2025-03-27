using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;

namespace Parking_Zone.Extensions
{
    public static class DbContextExtensions
    {
        public static IQueryable<Journal> Journals(this ApplicationDbContext context)
        {
            return context.Journals;
        }
    }
}
