using System;
using System.Security.Claims;
using System.Linq;

namespace Parking_Zone.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.Email);
            return claim?.Value;
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.Name);
            return claim?.Value;
        }

        public static string GetUserRole(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var claim = principal.FindFirst(ClaimTypes.Role);
            return claim?.Value;
        }

        public static bool IsInRole(this ClaimsPrincipal principal, params string[] roles)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            if (roles == null || !roles.Any())
                return false;

            return roles.Any(role => principal.IsInRole(role));
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Admin");
        }

        public static bool IsOperator(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Operator");
        }

        public static bool IsSupervisor(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Supervisor");
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var surname = principal.FindFirst(ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(givenName) && string.IsNullOrEmpty(surname))
                return principal.GetUserName();

            return $"{givenName} {surname}".Trim();
        }

        public static string GetAssignedGate(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var claim = principal.FindFirst("AssignedGate");
            return claim?.Value;
        }
    }
} 