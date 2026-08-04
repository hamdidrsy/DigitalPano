using System.Security.Claims;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;
using DigitalPano.Web.Models.Admin.Emergencies;
using DigitalPano.Web.Services;
using DigitalPano.Web.Services.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class EmergenciesController(
    AppDbContext dbContext,
    IInstitutionDateTimeService dateTimeService,
    TimeProvider timeProvider,
    IPanoNotifier? panoNotifier = null) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        EmergencyListItemViewModel[] items = await dbContext.Announcements.AsNoTracking()
            .Where(x => x.IsEmergency)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new EmergencyListItemViewModel(
                x.Id, x.Title, x.Description,
                dateTimeService.ToLocalTime(x.StartDateUtc),
                dateTimeService.ToLocalTime(x.EndDateUtc),
                x.IsActive && x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow,
                x.AnnouncementScreens.Select(s => s.Screen.Name).OrderBy(n => n).ToArray()))
            .ToArrayAsync(cancellationToken);
        return View(new EmergencyListViewModel { Items = items });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await PopulateOptionsAsync(new EmergencyFormViewModel(), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(EmergencyFormViewModel model, CancellationToken cancellationToken)
    {
        int[] screenIds = await dbContext.Screens.Where(x => model.SelectedScreenIds.Contains(x.Id) && x.IsActive)
            .Select(x => x.Id).ToArrayAsync(cancellationToken);
        if (screenIds.Length != model.SelectedScreenIds.Distinct().Count())
        {
            ModelState.AddModelError(nameof(model.SelectedScreenIds), "Seçilen ekranlardan biri bulunamadı veya pasif.");
        }

        string? imagePath = null;
        if (model.MediaId.HasValue)
        {
            imagePath = await dbContext.Media.Where(x => x.Id == model.MediaId && x.MediaType == MediaType.Image)
                .Select(x => x.RelativePath).SingleOrDefaultAsync(cancellationToken);
            if (imagePath is null) ModelState.AddModelError(nameof(model.MediaId), "Seçilen görsel bulunamadı.");
        }

        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        bool conflict = await dbContext.Announcements.AnyAsync(x => x.IsEmergency && x.IsActive &&
            x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow &&
            x.AnnouncementScreens.Any(s => screenIds.Contains(s.ScreenId)), cancellationToken);
        if (conflict)
        {
            ModelState.AddModelError(nameof(model.SelectedScreenIds), "Seçilen ekranlardan birinde zaten aktif bir acil yayın var.");
        }

        if (!ModelState.IsValid)
        {
            return View(await PopulateOptionsAsync(model, cancellationToken));
        }

        var emergency = new Announcement
        {
            Title = model.Title.Trim(), Description = model.Description.Trim(),
            ContentType = model.MediaId.HasValue ? AnnouncementContentType.Image : AnnouncementContentType.Text,
            MediaId = model.MediaId, StartDateUtc = utcNow, EndDateUtc = utcNow.AddMinutes(model.DurationMinutes),
            IsActive = true, IsEmergency = true, CreatedAtUtc = utcNow,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            AnnouncementScreens = screenIds.Select(id => new AnnouncementScreen { ScreenId = id }).ToList()
        };
        dbContext.Announcements.Add(emergency);
        await dbContext.SaveChangesAsync(cancellationToken);
        AddActivityLog("EmergencyStart", emergency, $"'{emergency.Title}' acil yayını başlatıldı.");
        await dbContext.SaveChangesAsync(cancellationToken);
        if (panoNotifier is not null) await panoNotifier.NotifyScreensAsync(screenIds, cancellationToken);
        TempData["SuccessMessage"] = "Acil duyuru hedef ekranlarda başlatıldı.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Stop(int id, bool confirm, CancellationToken cancellationToken)
    {
        if (!confirm)
        {
            TempData["ErrorMessage"] = "Acil yayını sonlandırmak için onay gereklidir.";
            return RedirectToAction(nameof(Index));
        }

        Announcement? emergency = await dbContext.Announcements.Include(x => x.AnnouncementScreens)
            .SingleOrDefaultAsync(x => x.Id == id && x.IsEmergency, cancellationToken);
        if (emergency is null) return NotFound();
        int[] screenIds = emergency.AnnouncementScreens.Select(x => x.ScreenId).ToArray();
        emergency.IsActive = false;
        emergency.EndDateUtc = timeProvider.GetUtcNow().UtcDateTime;
        emergency.UpdatedAtUtc = emergency.EndDateUtc;
        AddActivityLog("EmergencyStop", emergency, $"'{emergency.Title}' acil yayını sonlandırıldı.");
        await dbContext.SaveChangesAsync(cancellationToken);
        if (panoNotifier is not null) await panoNotifier.NotifyScreensAsync(screenIds, cancellationToken);
        TempData["SuccessMessage"] = "Acil yayın sonlandırıldı; normal yayın geri döndü.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<EmergencyFormViewModel> PopulateOptionsAsync(EmergencyFormViewModel model, CancellationToken token)
    {
        model.Screens = await dbContext.Screens.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name)
            .Select(x => new ScreenOptionViewModel(x.Id, x.Name, x.IsActive)).ToArrayAsync(token);
        model.ImageOptions = await dbContext.Media.AsNoTracking().Where(x => x.MediaType == MediaType.Image)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new MediaOptionViewModel(x.Id, x.OriginalFileName, x.MediaType)).ToArrayAsync(token);
        return model;
    }

    private void AddActivityLog(string action, Announcement item, string description) => dbContext.ActivityLogs.Add(new ActivityLog
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier), ActionType = action,
        EntityType = nameof(Announcement), EntityId = item.Id.ToString(System.Globalization.CultureInfo.InvariantCulture), Description = description,
        IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(), CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
    });
}
