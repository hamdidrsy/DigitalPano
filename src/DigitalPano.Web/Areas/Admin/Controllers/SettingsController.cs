using System.Security.Claims;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalPano.Web.Services.RealTime;

namespace DigitalPano.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class SettingsController(
    AppDbContext dbContext,
    TimeProvider timeProvider,
    IPanoNotifier? panoNotifier = null) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        InstitutionSetting? settings = await dbContext.InstitutionSettings
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        List<LogoOptionViewModel> logoOptions = await GetLogoOptionsAsync(cancellationToken);
        int? logoMediaId = settings?.LogoPath is null
            ? null
            : await dbContext.Media
                .Where(x => x.RelativePath == settings.LogoPath && x.MediaType == MediaType.Image)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

        return View(new InstitutionSettingsViewModel
        {
            InstitutionName = settings?.InstitutionName ?? "DigitalPano",
            LogoMediaId = logoMediaId,
            PrimaryColor = settings?.PrimaryColor ?? "#0D6EFD",
            SecondaryColor = settings?.SecondaryColor ?? "#6C757D",
            City = settings?.City ?? "İstanbul",
            LogoOptions = logoOptions
        });
    }

    [HttpPost]
    public async Task<IActionResult> Index(
        InstitutionSettingsViewModel model,
        CancellationToken cancellationToken)
    {
        string? logoPath = null;
        if (model.LogoMediaId.HasValue)
        {
            logoPath = await dbContext.Media
                .Where(x => x.Id == model.LogoMediaId && x.MediaType == MediaType.Image)
                .Select(x => x.RelativePath)
                .SingleOrDefaultAsync(cancellationToken);
            if (logoPath is null)
            {
                ModelState.AddModelError(nameof(model.LogoMediaId), "Seçilen logo görseli bulunamadı.");
            }
        }

        if (!ModelState.IsValid)
        {
            model.LogoOptions = await GetLogoOptionsAsync(cancellationToken);
            return View(model);
        }

        InstitutionSetting? settings = await dbContext.InstitutionSettings
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (settings is null)
        {
            settings = new InstitutionSetting();
            dbContext.InstitutionSettings.Add(settings);
        }

        settings.InstitutionName = model.InstitutionName.Trim();
        settings.LogoPath = logoPath;
        settings.PrimaryColor = model.PrimaryColor.ToUpperInvariant();
        settings.SecondaryColor = model.SecondaryColor.ToUpperInvariant();
        settings.City = model.City.Trim();
        settings.TimeZoneId = "Europe/Istanbul";
        settings.UpdatedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.ActivityLogs.Add(new ActivityLog
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ActionType = "Update",
            EntityType = nameof(InstitutionSetting),
            EntityId = settings.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Description = "Kurum ve tema ayarları güncellendi.",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        });
        await dbContext.SaveChangesAsync(cancellationToken);
        if (panoNotifier is not null)
        {
            await panoNotifier.NotifyAllAsync(cancellationToken);
        }

        TempData["SuccessMessage"] = "Kurum ayarları kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<LogoOptionViewModel>> GetLogoOptionsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Media
            .AsNoTracking()
            .Where(x => x.MediaType == MediaType.Image)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new LogoOptionViewModel(x.Id, x.OriginalFileName))
            .ToListAsync(cancellationToken);
    }
}
