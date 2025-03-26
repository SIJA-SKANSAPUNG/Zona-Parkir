using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IEnumerable<ApplicationUser>> GetOperatorsAsync();
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ActivateUserAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, string firstName, string lastName, string? photoPath);
        Task<bool> UpdateOperatorStatusAsync(string userId, bool isOperator);
        Task<bool> UpdateDutyStatusAsync(string userId, bool isOnDuty, DateTime? shiftStartTime = null, DateTime? shiftEndTime = null);
        Task<bool> AssignWorkstationAsync(string userId, int workstationId);
        Task<bool> UpdateAccessLevelAsync(string userId, string accessLevel);
    }
} 