using DigitalPano.Web.Areas.Admin.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Tickers;
using DigitalPano.Web.Services;
using DigitalPano.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class TickerMessageLifecycleTests
{
    [Fact]
    public async Task CreateEditAndDeletePersistTickerAndLogs()
    {
        await using AppDbContext dbContext = CreateContext();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.Zero));
        var httpContext = new DefaultHttpContext();
        var controller = new TickerMessagesController(dbContext, new InstitutionDateTimeService(), timeProvider)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };
        var createModel = new TickerMessageFormViewModel
        {
            Text = "  Yeni dönem kayıtları başladı.  ",
            StartDate = new DateTime(2026, 8, 5, 9, 0, 0),
            EndDate = new DateTime(2026, 8, 6, 18, 0, 0),
            SortOrder = 2,
            IsActive = true
        };

        IActionResult createResult = await controller.Create(createModel, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(createResult);
        TickerMessage message = await dbContext.TickerMessages.SingleAsync();
        Assert.Equal("Yeni dönem kayıtları başladı.", message.Text);
        Assert.Equal(new DateTime(2026, 8, 5, 6, 0, 0, DateTimeKind.Utc), message.StartDateUtc);

        IActionResult editResult = await controller.Edit(message.Id, new TickerMessageFormViewModel
        {
            Id = message.Id,
            Text = "Kayıt saatleri güncellendi.",
            StartDate = new DateTime(2026, 8, 5, 10, 0, 0),
            EndDate = new DateTime(2026, 8, 6, 19, 0, 0),
            SortOrder = 1,
            IsActive = false
        }, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(editResult);
        await dbContext.Entry(message).ReloadAsync();
        Assert.False(message.IsActive);
        Assert.Equal(1, message.SortOrder);

        IActionResult deleteResult = await controller.DeleteConfirmed(message.Id, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(deleteResult);
        Assert.Empty(await dbContext.TickerMessages.ToListAsync());
        Assert.Equal(3, await dbContext.ActivityLogs.CountAsync());
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"ticker-tests-{Guid.NewGuid():N}")
            .Options;
        return new AppDbContext(options);
    }
}
