using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Pano;
using DigitalPano.Web.Services;
using DigitalPano.Web.Services.Media;
using DigitalPano.Web.Services.Weather;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaEntity = DigitalPano.Web.Data.Entities.Media;

namespace DigitalPano.Web.Controllers;

[AllowAnonymous]
public sealed class PanoController(
    AppDbContext dbContext,
    IScreenKeyService screenKeyService,
    IMediaStorageService mediaStorageService,
    TimeProvider timeProvider,
    IWeatherService? weatherService = null) : Controller
{
    [HttpGet("pano/{slug}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Index(
        string slug,
        [FromQuery] string? key,
        CancellationToken cancellationToken)
    {
        Screen? screen = await GetAuthorizedScreenAsync(slug, key, cancellationToken);
        if (screen is null)
        {
            return NotFound();
        }

        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        InstitutionSetting? institution = await dbContext.InstitutionSettings
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);
        int? logoMediaId = institution?.LogoPath is null
            ? null
            : await dbContext.Media
                .Where(x => x.RelativePath == institution.LogoPath && x.MediaType == MediaType.Image)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        List<string> tickerMessages = await dbContext.TickerMessages
            .AsNoTracking()
            .Where(x => x.IsActive && x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow)
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Select(x => x.Text)
            .ToListAsync(cancellationToken);
        List<Announcement> announcements = await dbContext.Announcements
            .AsNoTracking()
            .Include(x => x.Media)
            .Where(x => x.IsActive &&
                        !x.IsEmergency &&
                        x.StartDateUtc <= utcNow &&
                        x.EndDateUtc >= utcNow &&
                        x.AnnouncementScreens.Any(s => s.ScreenId == screen.Id) &&
                        (x.ContentType == AnnouncementContentType.Text || x.MediaId != null))
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
        Announcement? emergency = await dbContext.Announcements
            .AsNoTracking()
            .Include(x => x.Media)
            .Where(x => x.IsActive && x.IsEmergency &&
                        x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow &&
                        x.AnnouncementScreens.Any(s => s.ScreenId == screen.Id) &&
                        (x.ContentType == AnnouncementContentType.Text || x.MediaId != null))
            .OrderByDescending(x => x.StartDateUtc)
            .FirstOrDefaultAsync(cancellationToken);
        string city = institution?.City ?? "İstanbul";
        WeatherSnapshot? weather = weatherService is null
            ? null
            : await weatherService.GetCurrentAsync(city, cancellationToken);

        var model = new PanoViewModel
        {
            InstitutionName = institution?.InstitutionName ?? "DigitalPano",
            LogoPath = institution?.LogoPath,
            LogoMediaId = logoMediaId,
            PrimaryColor = institution?.PrimaryColor ?? "#0D6EFD",
            SecondaryColor = institution?.SecondaryColor ?? "#6C757D",
            ScreenName = screen.Name,
            ScreenSlug = screen.Slug,
            DeviceKey = screen.DeviceKey,
            City = city,
            Weather = weather is null ? null : new PanoWeatherViewModel(
                weather.TemperatureCelsius, weather.Description, weather.Symbol),
            ContentCategories = announcements.Select(x => x.ContentType switch
                {
                    AnnouncementContentType.Image => "Görsel",
                    AnnouncementContentType.Video => "Video",
                    _ => "Metin"
                })
                .Distinct()
                .ToArray(),
            TickerMessages = tickerMessages,
            EmergencyContent = emergency is null ? null : new PanoContentItemViewModel(
                emergency.Id, emergency.Title, emergency.Description, emergency.ContentType,
                emergency.MediaId, emergency.Media?.MimeType, emergency.DisplayDurationSeconds),
            Items = announcements.Select(x => new PanoContentItemViewModel(
                x.Id,
                x.Title,
                x.Description,
                x.ContentType,
                x.MediaId,
                x.Media?.MimeType,
                x.DisplayDurationSeconds))
                .ToArray()
        };

        return View(model);
    }

    [HttpPost("pano/{slug}/heartbeat")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Heartbeat(
        string slug,
        [FromQuery] string? key,
        CancellationToken cancellationToken)
    {
        Screen? screen = await GetAuthorizedScreenAsync(slug, key, cancellationToken, track: true);
        if (screen is null)
        {
            return NotFound();
        }

        screen.LastConnectionDateUtc = timeProvider.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("pano/{slug}/medya/{mediaId:int}")]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> Media(
        string slug,
        int mediaId,
        [FromQuery] string? key,
        CancellationToken cancellationToken)
    {
        Screen? screen = await GetAuthorizedScreenAsync(slug, key, cancellationToken);
        if (screen is null)
        {
            return NotFound();
        }

        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        MediaEntity? media = await dbContext.Media
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.Id == mediaId &&
                (x.Announcements.Any(a =>
                     a.IsActive &&
                     a.StartDateUtc <= utcNow &&
                     a.EndDateUtc >= utcNow &&
                     a.AnnouncementScreens.Any(s => s.ScreenId == screen.Id)) ||
                 dbContext.InstitutionSettings.Any(s => s.LogoPath == x.RelativePath)),
                cancellationToken);
        if (media is null)
        {
            return NotFound();
        }

        Stream? stream = await mediaStorageService.OpenReadAsync(media.RelativePath, cancellationToken);
        if (stream is null)
        {
            return NotFound();
        }

        Response.Headers.XContentTypeOptions = "nosniff";
        return File(stream, media.MimeType, enableRangeProcessing: media.MediaType == MediaType.Video);
    }

    private async Task<Screen?> GetAuthorizedScreenAsync(
        string slug,
        string? suppliedKey,
        CancellationToken cancellationToken,
        bool track = false)
    {
        IQueryable<Screen> query = track ? dbContext.Screens : dbContext.Screens.AsNoTracking();
        Screen? screen = await query.SingleOrDefaultAsync(
            x => x.Slug == slug && x.IsActive,
            cancellationToken);
        return screen is not null && screenKeyService.IsValid(screen.DeviceKey, suppliedKey)
            ? screen
            : null;
    }
}
