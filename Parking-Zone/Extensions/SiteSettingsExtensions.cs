using Parking_Zone.Models;

namespace Parking_Zone.Extensions
{
    public static class SiteSettingsExtensions
    {
        public static string GetLogoUrl(this SiteSettings settings)
        {
            return settings.LogoPath ?? string.Empty;
        }

        public static string GetTheme(this SiteSettings settings)
        {
            return settings.ThemeName ?? "default";
        }

        public static string GetCurrencySymbol(this SiteSettings settings)
        {
            return settings.CurrencySymbol ?? "$";
        }

        public static string GetTimeFormat(this SiteSettings settings)
        {
            return settings.TimeFormat ?? "HH:mm";
        }

        public static string GetDateFormat(this SiteSettings settings)
        {
            return settings.DateFormat ?? "yyyy-MM-dd";
        }
    }
}
