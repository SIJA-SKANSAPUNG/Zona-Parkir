using Parking_Zone.Models;

namespace Parking_Zone.Services
{
    public interface ISiteSettingsService
    {
        Task<SiteSettings> GetSettingsAsync();
        Task<bool> UpdateSettingsAsync(SiteSettings settings);
        Task<bool> UpdateThemeAsync(string themeColor);
        Task<bool> ToggleLogoAsync(bool showLogo);
        Task<bool> ToggleNotificationsAsync(bool enableNotifications);
        Task<bool> ResetToDefaultsAsync();
    }
} 