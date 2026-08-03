using System.Security.Claims;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Tickers;
using DigitalPano.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class TickerMessagesController(
    AppDbContext dbContext,
    IInstitutionDateTimeService dateTimeService,
    TimeProvider timeProvider) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        List<TickerMessage> messages = await dbContext.TickerMessages
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenByDescending(x => x.StartDateUtc)
            .ToListAsync(cancellationToken);
        var model = new TickerMessageListViewModel
        {
            Items = messages.Select(x => new TickerMessageListItemViewModel(
                x.Id,
                x.Text,
                dateTimeService.ToLocalTime(x.StartDateUtc),
                dateTimeService.ToLocalTime(x.EndDateUtc),
                x.SortOrder,
                GetStatus(x, utcNow)))
                .ToArray()
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        DateTime localNow = dateTimeService.ToLocalTime(timeProvider.GetUtcNow().UtcDateTime);
        return View(new TickerMessageFormViewModel
        {
            StartDate = localNow,
            EndDate = localNow.AddDays(1)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TickerMessageFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var message = new TickerMessage
        {
            Text = model.Text.Trim(),
            StartDateUtc = dateTimeService.ToUtc(model.StartDate),
            EndDateUtc = dateTimeService.ToUtc(model.EndDate),
            SortOrder = model.SortOrder,
            IsActive = model.IsActive,
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };
        dbContext.TickerMessages.Add(message);
        await dbContext.SaveChangesAsync(cancellationToken);
        AddActivityLog("Create", message, "Kayan yazı oluşturuldu.");
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "Kayan yazı oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        TickerMessage? message = await dbContext.TickerMessages.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (message is null)
        {
            return NotFound();
        }

        return View(new TickerMessageFormViewModel
        {
            Id = message.Id,
            Text = message.Text,
            StartDate = dateTimeService.ToLocalTime(message.StartDateUtc),
            EndDate = dateTimeService.ToLocalTime(message.EndDateUtc),
            SortOrder = message.SortOrder,
            IsActive = message.IsActive
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, TickerMessageFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        TickerMessage? message = await dbContext.TickerMessages
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (message is null)
        {
            return NotFound();
        }

        message.Text = model.Text.Trim();
        message.StartDateUtc = dateTimeService.ToUtc(model.StartDate);
        message.EndDateUtc = dateTimeService.ToUtc(model.EndDate);
        message.SortOrder = model.SortOrder;
        message.IsActive = model.IsActive;
        AddActivityLog("Update", message, "Kayan yazı güncellendi.");
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "Kayan yazı güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        TickerMessage? message = await dbContext.TickerMessages.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return message is null ? NotFound() : View(message);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        TickerMessage? message = await dbContext.TickerMessages
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (message is null)
        {
            return NotFound();
        }

        AddActivityLog("Delete", message, "Kayan yazı silindi.");
        dbContext.TickerMessages.Remove(message);
        await dbContext.SaveChangesAsync(cancellationToken);
        TempData["SuccessMessage"] = "Kayan yazı silindi.";
        return RedirectToAction(nameof(Index));
    }

    private void AddActivityLog(string actionType, TickerMessage message, string description)
    {
        dbContext.ActivityLogs.Add(new ActivityLog
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            ActionType = actionType,
            EntityType = nameof(TickerMessage),
            EntityId = message.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
            Description = description,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        });
    }

    private static TickerMessageStatus GetStatus(TickerMessage message, DateTime utcNow)
    {
        if (!message.IsActive)
        {
            return TickerMessageStatus.Inactive;
        }

        if (message.EndDateUtc < utcNow)
        {
            return TickerMessageStatus.Expired;
        }

        return message.StartDateUtc > utcNow
            ? TickerMessageStatus.Scheduled
            : TickerMessageStatus.Active;
    }
}
