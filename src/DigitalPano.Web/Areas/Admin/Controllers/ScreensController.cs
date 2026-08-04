using System.Security.Claims;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Screens;
using DigitalPano.Web.Services;
using DigitalPano.Web.Services.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class ScreensController(
    AppDbContext dbContext,
    ISlugService slugService,
    IScreenKeyService screenKeyService,
    IInstitutionDateTimeService dateTimeService,
    TimeProvider timeProvider,
    IPanoNotifier? panoNotifier = null) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        DateTime onlineThreshold = timeProvider.GetUtcNow().UtcDateTime.AddMinutes(-2);
        List<ScreenListItemViewModel> items = await dbContext.Screens
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ScreenListItemViewModel(
                x.Id,
                x.Name,
                x.Slug,
                x.DeviceKey,
                x.Location,
                x.IsActive,
                x.IsActive && x.LastConnectionDateUtc >= onlineThreshold,
                x.LastConnectionDateUtc.HasValue
                    ? dateTimeService.ToLocalTime(x.LastConnectionDateUtc.Value)
                    : null,
                x.AnnouncementScreens.Count))
            .ToListAsync(cancellationToken);

        return View(new ScreenListViewModel { Items = items });
    }

    [HttpGet]
    public IActionResult Create() => View(new ScreenFormViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(ScreenFormViewModel model, CancellationToken cancellationToken)
    {
        string slug = slugService.CreateSlug(string.IsNullOrWhiteSpace(model.Slug) ? model.Name : model.Slug);
        await ValidateSlugAsync(slug, excludedScreenId: null, cancellationToken);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var screen = new Screen
        {
            Name = model.Name.Trim(),
            Slug = slug,
            DeviceKey = screenKeyService.Generate(),
            Location = NormalizeOptional(model.Location),
            IsActive = model.IsActive,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };
        dbContext.Screens.Add(screen);
        await dbContext.SaveChangesAsync(cancellationToken);
        AddActivityLog("Create", screen, $"'{screen.Name}' ekranı oluşturuldu.");
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Ekran başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        Screen? screen = await dbContext.Screens.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (screen is null)
        {
            return NotFound();
        }

        return View(new ScreenFormViewModel
        {
            Id = screen.Id,
            Name = screen.Name,
            Slug = screen.Slug,
            Location = screen.Location,
            IsActive = screen.IsActive
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ScreenFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        string slug = slugService.CreateSlug(string.IsNullOrWhiteSpace(model.Slug) ? model.Name : model.Slug);
        await ValidateSlugAsync(slug, id, cancellationToken);
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Screen? screen = await dbContext.Screens.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (screen is null)
        {
            return NotFound();
        }

        screen.Name = model.Name.Trim();
        screen.Slug = slug;
        screen.Location = NormalizeOptional(model.Location);
        screen.IsActive = model.IsActive;
        AddActivityLog("Update", screen, $"'{screen.Name}' ekranı güncellendi.");
        await dbContext.SaveChangesAsync(cancellationToken);
        if (panoNotifier is not null) await panoNotifier.NotifyScreensAsync([screen.Id], cancellationToken);

        TempData["SuccessMessage"] = "Ekran başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RegenerateKey(int id, CancellationToken cancellationToken)
    {
        Screen? screen = await dbContext.Screens.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (screen is null)
        {
            return NotFound();
        }

        screen.DeviceKey = screenKeyService.Generate();
        AddActivityLog("RegenerateKey", screen, $"'{screen.Name}' ekranının cihaz anahtarı yenilendi.");
        await dbContext.SaveChangesAsync(cancellationToken);
        if (panoNotifier is not null) await panoNotifier.NotifyScreensAsync([screen.Id], cancellationToken);

        TempData["SuccessMessage"] = "Cihaz anahtarı yenilendi. Eski pano adresi artık çalışmaz.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        ScreenDeleteViewModel? model = await dbContext.Screens
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new ScreenDeleteViewModel(x.Id, x.Name, x.Slug, x.AnnouncementScreens.Count))
            .SingleOrDefaultAsync(cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        Screen? screen = await dbContext.Screens
            .Include(x => x.AnnouncementScreens)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (screen is null)
        {
            return NotFound();
        }

        if (screen.AnnouncementScreens.Count > 0)
        {
            TempData["ErrorMessage"] = "Duyuru atanmış ekran silinemez; önce ekranı pasif yapabilirsiniz.";
            return RedirectToAction(nameof(Index));
        }

        AddActivityLog("Delete", screen, $"'{screen.Name}' ekranı silindi.");
        dbContext.Screens.Remove(screen);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Ekran silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateSlugAsync(
        string slug,
        int? excludedScreenId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            ModelState.AddModelError(nameof(ScreenFormViewModel.Slug), "Geçerli bir adres anahtarı üretilemedi.");
            return;
        }

        bool exists = await dbContext.Screens.AnyAsync(
            x => x.Slug == slug && (!excludedScreenId.HasValue || x.Id != excludedScreenId.Value),
            cancellationToken);
        if (exists)
        {
            ModelState.AddModelError(nameof(ScreenFormViewModel.Slug), "Bu adres anahtarı başka bir ekranda kullanılıyor.");
        }
    }

    private void AddActivityLog(string actionType, Screen screen, string description)
    {
        dbContext.ActivityLogs.Add(new ActivityLog
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ActionType = actionType,
            EntityType = nameof(Screen),
            EntityId = screen.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Description = description,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        });
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
