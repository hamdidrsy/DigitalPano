using DigitalPano.Web.Areas.Admin.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;
using DigitalPano.Web.Services;
using DigitalPano.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class AnnouncementLifecycleTests
{
    [Fact]
    public async Task CreateEditAndDeletePersistRelationsAndActivityLogs()
    {
        await using AppDbContext dbContext = CreateContext();
        dbContext.Screens.Add(new Screen
        {
            Id = 10,
            Name = "Test Ekranı",
            Slug = "test-ekrani",
            DeviceKey = "test-device-key",
            IsActive = true
        });
        await dbContext.SaveChangesAsync();

        var timeProvider = new FixedTimeProvider(
            new DateTimeOffset(2026, 8, 4, 9, 0, 0, TimeSpan.Zero));
        var dateTimeService = new InstitutionDateTimeService();
        var httpContext = new DefaultHttpContext();
        var controller = new AnnouncementsController(
            dbContext,
            new AnnouncementStatusService(timeProvider),
            dateTimeService,
            timeProvider)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };

        var createModel = new AnnouncementFormViewModel
        {
            Title = "  Sınav duyurusu  ",
            Description = "  Sınav saat 10.00'da başlayacaktır.  ",
            StartDate = new DateTime(2026, 8, 5, 10, 0, 0),
            EndDate = new DateTime(2026, 8, 5, 12, 0, 0),
            DisplayDurationSeconds = 15,
            SortOrder = 2,
            IsActive = true,
            SelectedScreenIds = [10]
        };

        IActionResult createResult = await controller.Create(createModel, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(createResult);
        Announcement created = await dbContext.Announcements
            .Include(x => x.AnnouncementScreens)
            .SingleAsync();
        Assert.Equal("Sınav duyurusu", created.Title);
        Assert.Equal(new DateTime(2026, 8, 5, 7, 0, 0, DateTimeKind.Utc), created.StartDateUtc);
        Assert.Equal(10, Assert.Single(created.AnnouncementScreens).ScreenId);

        var editModel = new AnnouncementFormViewModel
        {
            Id = created.Id,
            Title = "Güncel sınav duyurusu",
            Description = "Sınav saati güncellendi.",
            StartDate = new DateTime(2026, 8, 5, 11, 0, 0),
            EndDate = new DateTime(2026, 8, 5, 13, 0, 0),
            DisplayDurationSeconds = 20,
            SortOrder = 1,
            IsActive = false,
            SelectedScreenIds = [10]
        };

        IActionResult editResult = await controller.Edit(created.Id, editModel, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(editResult);
        await dbContext.Entry(created).ReloadAsync();
        Assert.Equal("Güncel sınav duyurusu", created.Title);
        Assert.False(created.IsActive);
        Assert.Equal(20, created.DisplayDurationSeconds);

        IActionResult deleteResult = await controller.DeleteConfirmed(created.Id, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(deleteResult);
        Assert.Empty(await dbContext.Announcements.ToListAsync());
        Assert.Equal(3, await dbContext.ActivityLogs.CountAsync());
    }

    [Fact]
    public async Task CreateRejectsUnknownScreen()
    {
        await using AppDbContext dbContext = CreateContext();
        var timeProvider = new FixedTimeProvider(
            new DateTimeOffset(2026, 8, 4, 9, 0, 0, TimeSpan.Zero));
        var httpContext = new DefaultHttpContext();
        var controller = new AnnouncementsController(
            dbContext,
            new AnnouncementStatusService(timeProvider),
            new InstitutionDateTimeService(),
            timeProvider)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };
        var model = new AnnouncementFormViewModel
        {
            Title = "Duyuru",
            Description = "Açıklama",
            StartDate = new DateTime(2026, 8, 5, 10, 0, 0),
            EndDate = new DateTime(2026, 8, 5, 12, 0, 0),
            SelectedScreenIds = [999]
        };

        IActionResult result = await controller.Create(model, CancellationToken.None);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.Empty(await dbContext.Announcements.ToListAsync());
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"announcement-tests-{Guid.NewGuid():N}")
            .Options;

        return new AppDbContext(options);
    }
}

internal sealed class TestTempDataProvider : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) =>
        new Dictionary<string, object>();

    public void SaveTempData(HttpContext context, IDictionary<string, object> values)
    {
    }
}
