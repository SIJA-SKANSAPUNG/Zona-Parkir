using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Parking_Zone.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserAgent(this HttpContext context)
        {
            return context.Request.Headers["User-Agent"].ToString();
        }

        public static string GetIpAddress(this HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public static bool IsAjaxRequest(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   context.Request.Headers.Accept.Any(x => x.Contains("application/json"));
        }

        public static bool IsMobileDevice(this HttpContext context)
        {
            var userAgent = context.GetUserAgent().ToLower();
            return userAgent.Contains("mobile") || userAgent.Contains("android") || userAgent.Contains("iphone");
        }

        public static void SetSessionData<T>(this HttpContext context, string key, T value)
        {
            if (context.Session == null)
                throw new InvalidOperationException("Session is not enabled");

            var json = JsonSerializer.Serialize(value);
            context.Session.SetString(key, json);
        }

        public static T GetSessionData<T>(this HttpContext context, string key)
        {
            if (context.Session == null)
                throw new InvalidOperationException("Session is not enabled");

            var json = context.Session.GetString(key);
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public static void SetCookie(this HttpContext context, string key, string value, int? expireTime = null)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = context.Request.IsHttps
            };

            if (expireTime.HasValue)
                options.Expires = DateTime.Now.AddMinutes(expireTime.Value);

            context.Response.Cookies.Append(key, value, options);
        }

        public static string GetCookie(this HttpContext context, string key)
        {
            return context.Request.Cookies[key];
        }

        public static void DeleteCookie(this HttpContext context, string key)
        {
            context.Response.Cookies.Delete(key);
        }

        public static string GetCurrentUserId(this HttpContext context)
        {
            return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetCurrentUserName(this HttpContext context)
        {
            return context.User?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetCurrentUserEmail(this HttpContext context)
        {
            return context.User?.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static bool IsAuthenticated(this HttpContext context)
        {
            return context.User?.Identity?.IsAuthenticated ?? false;
        }

        public static bool IsInRole(this HttpContext context, string role)
        {
            return context.User?.IsInRole(role) ?? false;
        }

        public static string GetRequestScheme(this HttpContext context)
        {
            return context.Request.Scheme;
        }

        public static string GetBaseUrl(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}";
        }

        public static string GetCurrentUrl(this HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";
        }
    }
} 