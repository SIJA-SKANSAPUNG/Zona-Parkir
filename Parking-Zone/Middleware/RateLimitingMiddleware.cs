using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Parking_Zone.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentDictionary<string, DateTime> _lastRequestTimes = new();
        private static readonly TimeSpan _requestInterval = TimeSpan.FromMilliseconds(100);
        
        private static readonly string[] _excludedPaths = new[] 
        { 
            "/api/vehicles",
            "/api/gates",
            "/api/transactions",
            "/Auth/Login",
            "/Auth/Logout",
            "/Dashboard",
            "/Reports",
            "/Gates/Entry",
            "/Gates/Exit",
            "/static/",
            "/lib/",
            "/css/",
            "/js/",
            "/images/",
            "/favicon.ico"
        };

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? "";
            
            if (_excludedPaths.Any(p => path.Contains(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }
            
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var currentTime = DateTime.UtcNow;

            if (_lastRequestTimes.TryGetValue(ipAddress, out var lastRequestTime))
            {
                var timeSinceLastRequest = currentTime - lastRequestTime;
                if (timeSinceLastRequest < _requestInterval)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                    return;
                }
            }

            _lastRequestTimes.AddOrUpdate(ipAddress, currentTime, (_, _) => currentTime);
            await _next(context);
        }
    }
}
