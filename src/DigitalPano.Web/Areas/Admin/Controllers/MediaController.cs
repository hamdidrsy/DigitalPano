using System.Security.Claims;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Media;
using DigitalPano.Web.Services;
using DigitalPano.Web.Services.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediaEntity = DigitalPano.Web.Data.Entities.Media;

namespace DigitalPano.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class MediaController(
    AppDbContext dbContext,
    IMediaStorageService storageService,
    IInstitutionDateTimeService dateTimeService,
    TimeProvider timeProvider) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        List<MediaListItemViewModel> items = await dbContext.Media
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new MediaListItemViewModel(
                x.Id,
                x.OriginalFileName,
                x.MimeType,
                x.FileSize,
                x.MediaType,
                dateTimeService.ToLocalTime(x.CreatedAtUtc),
                x.Announcements.Count))
            .ToListAsync(cancellationToken);

        return View(new MediaListViewModel { Items = items });
    }

    [HttpGet]
    public IActionResult Upload() => View(new MediaUploadViewModel());

    [HttpPost]
    [RequestSizeLimit(220L * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        MediaUploadViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || model.File is null)
        {
            return View(model);
        }

        MediaValidationResult validation = await storageService.ValidateAsync(model.File, cancellationToken);
        if (!validation.IsValid)
        {
            ModelState.AddModelError(nameof(model.File), validation.ErrorMessage!);
            return View(model);
        }

        StoredMediaFile storedFile = await storageService.StoreAsync(model.File, validation, cancellationToken);
        var media = new MediaEntity
        {
            OriginalFileName = Path.GetFileName(model.File.FileName),
            StoredFileName = storedFile.StoredFileName,
            RelativePath = storedFile.RelativePath,
            MimeType = storedFile.MimeType,
            FileSize = storedFile.FileSize,
            MediaType = storedFile.MediaType,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        try
        {
            dbContext.Media.Add(media);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await storageService.DeleteAsync(storedFile.RelativePath, CancellationToken.None);
            throw;
        }

        AddActivityLog("Upload", media, $"'{media.OriginalFileName}' medya dosyası yüklendi.");
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Medya dosyası başarıyla yüklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Preview(int id, CancellationToken cancellationToken)
    {
        MediaEntity? media = await dbContext.Media
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (media is null)
        {
            return NotFound();
        }

        Stream? stream = await storageService.OpenReadAsync(media.RelativePath, cancellationToken);
        if (stream is null)
        {
            return NotFound();
        }

        Response.Headers.XContentTypeOptions = "nosniff";
        Response.Headers.CacheControl = "private, max-age=300";
        return File(stream, media.MimeType, enableRangeProcessing: media.MediaType == MediaType.Video);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        MediaDeleteViewModel? model = await dbContext.Media
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new MediaDeleteViewModel(
                x.Id,
                x.OriginalFileName,
                x.MediaType,
                x.FileSize,
                x.Announcements.Count))
            .SingleOrDefaultAsync(cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        MediaEntity? media = await dbContext.Media
            .Include(x => x.Announcements)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (media is null)
        {
            return NotFound();
        }

        if (media.Announcements.Count > 0)
        {
            TempData["ErrorMessage"] = "Bir duyuruda kullanılan medya dosyası silinemez.";
            return RedirectToAction(nameof(Index));
        }

        await storageService.DeleteAsync(media.RelativePath, cancellationToken);
        AddActivityLog("Delete", media, $"'{media.OriginalFileName}' medya dosyası silindi.");
        dbContext.Media.Remove(media);
        await dbContext.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Medya dosyası silindi.";
        return RedirectToAction(nameof(Index));
    }

    private void AddActivityLog(string actionType, MediaEntity media, string description)
    {
        dbContext.ActivityLogs.Add(new ActivityLog
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ActionType = actionType,
            EntityType = nameof(MediaEntity),
            EntityId = media.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Description = description,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        });
    }
}
