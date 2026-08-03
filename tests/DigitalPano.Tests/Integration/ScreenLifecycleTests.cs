using DigitalPano.Web.Areas.Admin.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Screens;
using DigitalPano.Web.Services;
using DigitalPano.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class ScreenLifecycleTests
{
    [Fact]
    public async Task CreateEditRegenerateKeyAndDeletePersistExpectedChanges()
    {
        await using AppDbContext dbContext = CreateContext();
        var timeProvider = new FixedTimeProvider(
            new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.Zero));
        var httpContext = new DefaultHttpContext();
        var controller = new ScreensController(
            dbContext,
            new SlugService(),
            new ScreenKeyService(),
            new InstitutionDateTimeService(),
            timeProvider)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };

        IActionResult createResult = await controller.Create(new ScreenFormViewModel
        {
            Name = "Öğretmenler Odası",
            Location = "1. Kat",
            IsActive = true
        }, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(createResult);
        Screen screen = await dbContext.Screens.SingleAsync();
        Assert.Equal("ogretmenler-odasi", screen.Slug);
        Assert.Equal(64, screen.DeviceKey.Length);
        string originalKey = screen.DeviceKey;

        IActionResult editResult = await controller.Edit(screen.Id, new ScreenFormViewModel
        {
            Id = screen.Id,
            Name = "Öğretmen Ekranı",
            Slug = "ogretmen-ekrani",
            Location = "2. Kat",
            IsActive = false
        }, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(editResult);
        await dbContext.Entry(screen).ReloadAsync();
        Assert.Equal("Öğretmen Ekranı", screen.Name);
        Assert.False(screen.IsActive);

        IActionResult keyResult = await controller.RegenerateKey(screen.Id, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(keyResult);
        await dbContext.Entry(screen).ReloadAsync();
        Assert.NotEqual(originalKey, screen.DeviceKey);

        IActionResult deleteResult = await controller.DeleteConfirmed(screen.Id, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(deleteResult);
        Assert.Empty(await dbContext.Screens.ToListAsync());
        Assert.Equal(4, await dbContext.ActivityLogs.CountAsync());
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"screen-tests-{Guid.NewGuid():N}")
            .Options;
        return new AppDbContext(options);
    }
}
