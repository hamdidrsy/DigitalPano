using System.Security.Claims;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;
using DigitalPano.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class AnnouncementsController(
    AppDbContext dbContext,
    IAnnouncementStatusService statusService,
    IInstitutionDateTimeService dateTimeService,
    TimeProvider timeProvider) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        string? search,
        AnnouncementStatus? status,
        int? screenId,
        CancellationToken cancellationToken)
    {
        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        IQueryable<Announcement> query = dbContext.Announcements
            .AsNoTracking()
            .Include(x => x.AnnouncementScreens)
                .ThenInclude(x => x.Screen);

        if (!string.IsNullOrWhiteSpace(search))
        {
            string normalizedSearch = search.Trim();
            query = query.Where(x =>
                x.Title.Contains(normalizedSearch) || x.Description.Contains(normalizedSearch));
        }

        if (screenId.HasValue)
        {
            query = query.Where(x => x.AnnouncementScreens.Any(s => s.ScreenId == screenId.Value));
        }

        query = status switch
        {
            AnnouncementStatus.Active => query.Where(x =>
                x.IsActive && x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow),
            AnnouncementStatus.Scheduled => query.Where(x => x.IsActive && x.StartDateUtc > utcNow),
            AnnouncementStatus.Expired => query.Where(x => x.IsActive && x.EndDateUtc < utcNow),
            AnnouncementStatus.Inactive => query.Where(x => !x.IsActive),
            _ => query
        };

        List<Announcement> announcements = await query
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.StartDateUtc)
            .ToListAsync(cancellationToken);

        var model = new AnnouncementListViewModel
        {
            Search = search?.Trim(),
            Status = status,
            ScreenId = screenId,
            Screens = await GetScreenOptionsAsync(cancellationToken),
            Items = announcements.Select(x => new AnnouncementListItemViewModel(
                x.Id,
                x.Title,
                x.Description,
                dateTimeService.ToLocalTime(x.StartDateUtc),
                dateTimeService.ToLocalTime(x.EndDateUtc),
                x.DisplayDurationSeconds,
                x.SortOrder,
                statusService.GetStatus(x),
                x.AnnouncementScreens.Select(s => s.Screen.Name).OrderBy(name => name).ToArray()))
                .ToArray()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        DateTime localNow = dateTimeService.ToLocalTime(timeProvider.GetUtcNow().UtcDateTime);
        var model = new AnnouncementFormViewModel
        {
            StartDate = RoundToNextFiveMinutes(localNow),
            EndDate = RoundToNextFiveMinutes(localNow).AddDays(1),
            Screens = await GetScreenOptionsAsync(cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        AnnouncementFormViewModel model,
        CancellationToken cancellationToken)
    {
        List<int> validScreenIds = await ValidateAndGetScreenIdsAsync(model, cancellationToken);
        if (!ModelState.IsValid)
        {
            model.Screens = await GetScreenOptionsAsync(cancellationToken);
            return View(model);
        }

        var announcement = new Announcement
        {
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            ContentType = AnnouncementContentType.Text,
            StartDateUtc = dateTimeService.ToUtc(model.StartDate),
            EndDateUtc = dateTimeService.ToUtc(model.EndDate),
            DisplayDurationSeconds = model.DisplayDurationSeconds,
            SortOrder = model.SortOrder,
            IsActive = model.IsActive,
            IsEmergency = false,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime,
            CreatedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            AnnouncementScreens = validScreenIds
                .Select(id => new AnnouncementScreen { ScreenId = id })
                .ToList()
        };

        dbContext.Announcements.Add(announcement);
        await dbContext.SaveChangesAsync(cancellationToken);

        AddActivityLog("Create", announcement, $"'{announcement.Title}' duyurusu oluşturuldu.");
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Duyuru başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        Announcement? announcement = await dbContext.Announcements
            .AsNoTracking()
            .Include(x => x.AnnouncementScreens)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (announcement is null)
        {
            return NotFound();
        }

        var model = new AnnouncementFormViewModel
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Description = announcement.Description,
            StartDate = dateTimeService.ToLocalTime(announcement.StartDateUtc),
            EndDate = dateTimeService.ToLocalTime(announcement.EndDateUtc),
            DisplayDurationSeconds = announcement.DisplayDurationSeconds,
            SortOrder = announcement.SortOrder,
            IsActive = announcement.IsActive,
            SelectedScreenIds = announcement.AnnouncementScreens.Select(x => x.ScreenId).ToList(),
            Screens = await GetScreenOptionsAsync(cancellationToken)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        int id,
        AnnouncementFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        List<int> validScreenIds = await ValidateAndGetScreenIdsAsync(model, cancellationToken);
        if (!ModelState.IsValid)
        {
            model.Screens = await GetScreenOptionsAsync(cancellationToken);
            return View(model);
        }

        Announcement? announcement = await dbContext.Announcements
            .Include(x => x.AnnouncementScreens)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (announcement is null)
        {
            return NotFound();
        }

        announcement.Title = model.Title.Trim();
        announcement.Description = model.Description.Trim();
        announcement.StartDateUtc = dateTimeService.ToUtc(model.StartDate);
        announcement.EndDateUtc = dateTimeService.ToUtc(model.EndDate);
        announcement.DisplayDurationSeconds = model.DisplayDurationSeconds;
        announcement.SortOrder = model.SortOrder;
        announcement.IsActive = model.IsActive;
        announcement.UpdatedAtUtc = timeProvider.GetUtcNow().UtcDateTime;

        HashSet<int> selectedScreenIds = validScreenIds.ToHashSet();
        List<AnnouncementScreen> removedScreens = announcement.AnnouncementScreens
            .Where(x => !selectedScreenIds.Contains(x.ScreenId))
            .ToList();
        dbContext.AnnouncementScreens.RemoveRange(removedScreens);

        HashSet<int> existingScreenIds = announcement.AnnouncementScreens
            .Select(x => x.ScreenId)
            .ToHashSet();
        foreach (int selectedScreenId in selectedScreenIds.Except(existingScreenIds))
        {
            announcement.AnnouncementScreens.Add(new AnnouncementScreen
            {
                AnnouncementId = announcement.Id,
                ScreenId = selectedScreenId
            });
        }

        AddActivityLog("Update", announcement, $"'{announcement.Title}' duyurusu güncellendi.");
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Duyuru başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Preview(int id, CancellationToken cancellationToken)
    {
        Announcement? announcement = await dbContext.Announcements
            .AsNoTracking()
            .Include(x => x.AnnouncementScreens)
                .ThenInclude(x => x.Screen)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (announcement is null)
        {
            return NotFound();
        }

        var model = new AnnouncementPreviewViewModel(
            announcement.Id,
            announcement.Title,
            announcement.Description,
            dateTimeService.ToLocalTime(announcement.StartDateUtc),
            dateTimeService.ToLocalTime(announcement.EndDateUtc),
            announcement.DisplayDurationSeconds,
            statusService.GetStatus(announcement),
            announcement.AnnouncementScreens.Select(x => x.Screen.Name).OrderBy(x => x).ToArray());

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        Announcement? announcement = await dbContext.Announcements
            .AsNoTracking()
            .Include(x => x.AnnouncementScreens)
                .ThenInclude(x => x.Screen)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (announcement is null)
        {
            return NotFound();
        }

        return View(new AnnouncementDeleteViewModel(
            announcement.Id,
            announcement.Title,
            dateTimeService.ToLocalTime(announcement.StartDateUtc),
            dateTimeService.ToLocalTime(announcement.EndDateUtc),
            announcement.AnnouncementScreens.Select(x => x.Screen.Name).OrderBy(x => x).ToArray()));
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        Announcement? announcement = await dbContext.Announcements
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (announcement is null)
        {
            return NotFound();
        }

        AddActivityLog("Delete", announcement, $"'{announcement.Title}' duyurusu silindi.");
        dbContext.Announcements.Remove(announcement);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Duyuru silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<IReadOnlyList<ScreenOptionViewModel>> GetScreenOptionsAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Screens
            .AsNoTracking()
            .OrderByDescending(x => x.IsActive)
            .ThenBy(x => x.Name)
            .Select(x => new ScreenOptionViewModel(x.Id, x.Name, x.IsActive))
            .ToListAsync(cancellationToken);
    }

    private async Task<List<int>> ValidateAndGetScreenIdsAsync(
        AnnouncementFormViewModel model,
        CancellationToken cancellationToken)
    {
        List<int> requestedIds = model.SelectedScreenIds.Distinct().ToList();
        List<int> validIds = await dbContext.Screens
            .Where(x => requestedIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (requestedIds.Count > 0 && validIds.Count != requestedIds.Count)
        {
            ModelState.AddModelError(nameof(model.SelectedScreenIds), "Seçilen ekranlardan biri bulunamadı.");
        }

        return validIds;
    }

    private void AddActivityLog(string actionType, Announcement announcement, string description)
    {
        dbContext.ActivityLogs.Add(new ActivityLog
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ActionType = actionType,
            EntityType = nameof(Announcement),
            EntityId = announcement.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Description = description,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        });
    }

    private static DateTime RoundToNextFiveMinutes(DateTime value)
    {
        DateTime withoutSeconds = new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        int minutesToAdd = 5 - (withoutSeconds.Minute % 5);
        return withoutSeconds.AddMinutes(minutesToAdd);
    }
}
