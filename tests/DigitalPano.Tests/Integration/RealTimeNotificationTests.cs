using DigitalPano.Tests.Services;
using DigitalPano.Web.Areas.Admin.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;
using DigitalPano.Web.Services;
using DigitalPano.Web.Services.RealTime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class RealTimeNotificationTests
{
    [Fact]
    public async Task AnnouncementCreateNotifiesOnlySelectedScreens()
    {
        await using AppDbContext dbContext = CreateContext();
        dbContext.Screens.AddRange(
            new Screen { Id = 1, Name = "Giriş", Slug = "giris", DeviceKey = "key-1" },
            new Screen { Id = 2, Name = "Kantin", Slug = "kantin", DeviceKey = "key-2" });
        await dbContext.SaveChangesAsync();
        var notifier = new RecordingPanoNotifier();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 8, 4, 9, 0, 0, TimeSpan.Zero));
        var controller = Prepare(new AnnouncementsController(
            dbContext, new AnnouncementStatusService(timeProvider),
            new InstitutionDateTimeService(), timeProvider, notifier));

        await controller.Create(new AnnouncementFormViewModel
        {
            Title = "Anlık duyuru",
            Description = "İçerik",
            StartDate = new DateTime(2026, 8, 4, 12, 0, 0),
            EndDate = new DateTime(2026, 8, 4, 13, 0, 0),
            SelectedScreenIds = [2]
        }, CancellationToken.None);

        Assert.Equal([2], Assert.Single(notifier.ScreenNotifications));
        Assert.Equal(0, notifier.AllNotificationCount);
    }

    [Fact]
    public void ScreenGroupsAreStableAndCultureIndependent()
    {
        Assert.Equal("pano:42", PanoGroups.ForScreen(42));
        Assert.Equal("pano:all", PanoGroups.AllScreens);
    }

    private static T Prepare<T>(T controller) where T : Controller
    {
        var context = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext { HttpContext = context };
        controller.TempData = new TempDataDictionary(context, new TestTempDataProvider());
        return controller;
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"realtime-tests-{Guid.NewGuid():N}").Options;
        return new AppDbContext(options);
    }
}

internal sealed class RecordingPanoNotifier : IPanoNotifier
{
    public List<int[]> ScreenNotifications { get; } = [];
    public int AllNotificationCount { get; private set; }

    public Task NotifyScreensAsync(IEnumerable<int> screenIds, CancellationToken cancellationToken = default)
    {
        ScreenNotifications.Add(screenIds.OrderBy(x => x).ToArray());
        return Task.CompletedTask;
    }

    public Task NotifyAllAsync(CancellationToken cancellationToken = default)
    {
        AllNotificationCount++;
        return Task.CompletedTask;
    }
}
