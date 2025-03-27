using Microsoft.AspNetCore.Identity;
using System;

namespace Parking_Zone.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}".Trim();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FirstName = string.Empty;
                    LastName = string.Empty;
                    return;
                }
                
                var parts = value.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                FirstName = parts.Length > 0 ? parts[0] : string.Empty;
                LastName = parts.Length > 1 ? parts[1] : string.Empty;
            }
        }
    }
}
