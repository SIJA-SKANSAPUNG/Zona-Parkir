using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parking_Zone.Data;
using Parking_Zone.Models;
using Parking_Zone.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Parking_Zone.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SiteSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISiteSettingsService _settingsService;

        public SiteSettingsController(
            ApplicationDbContext context,
            ISiteSettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.SiteSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new SiteSettings
                {
                    SiteName = "ParkIRC",
                    ShowLogo = true,
                    ThemeColor = "#007bff",
                    FooterText = " 2024 ParkIRC"
                };
                await _context.SiteSettings.AddAsync(settings);
                await _context.SaveChangesAsync();
            }
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SiteSettings model, IFormFile logo, IFormFile favicon)
        {
            if (ModelState.IsValid)
            {
                var settings = await _context.SiteSettings.FirstAsync();
                
                if (logo != null)
                {
                    var logoPath = Path.Combine("images", "site", $"logo_{DateTime.Now:yyyyMMddHHmmss}.png");
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, logoPath);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }
                    settings.LogoPath = "/" + logoPath.Replace("\\", "/");
                }

                if (favicon != null)
                {
                    var faviconPath = Path.Combine("images", "site", $"favicon_{DateTime.Now:yyyyMMddHHmmss}.ico");
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, faviconPath);
                    
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await favicon.CopyToAsync(stream);
                    }
                    settings.FaviconPath = "/" + faviconPath.Replace("\\", "/");
                }

                settings.SiteName = model.SiteName;
                settings.FooterText = model.FooterText;
                settings.ThemeColor = model.ThemeColor;
                settings.ShowLogo = model.ShowLogo;
                settings.LastUpdated = DateTime.Now;
                settings.UpdatedBy = User.Identity.Name;

                _context.Update(settings);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Pengaturan site berhasil diupdate";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}