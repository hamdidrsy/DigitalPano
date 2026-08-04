using DigitalPano.Tests.Services;
using DigitalPano.Web.Areas.Admin.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Emergencies;
using DigitalPano.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class EmergencyLifecycleTests
{
    [Fact]
    public async Task StartBlocksSecondActiveEmergencyAndStopRestoresScreen()
    {
        await using AppDbContext dbContext = CreateContext();
        dbContext.Screens.Add(new Screen { Id = 7, Name = "Giriş", Slug = "giris", DeviceKey = "key", IsActive = true });
        await dbContext.SaveChangesAsync();
        var notifier = new RecordingPanoNotifier();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 8, 4, 9, 0, 0, TimeSpan.Zero));
        var context = new DefaultHttpContext();
        var controller = new EmergenciesController(dbContext, new InstitutionDateTimeService(), timeProvider, notifier)
        {
            ControllerContext = new ControllerContext { HttpContext = context },
            TempData = new TempDataDictionary(context, new TestTempDataProvider())
        };

        IActionResult started = await controller.Create(CreateModel("Yangın tatbikatı"), CancellationToken.None);
        Assert.IsType<RedirectToActionResult>(started);
        Announcement emergency = await dbContext.Announcements.Include(x => x.AnnouncementScreens).SingleAsync();
        Assert.True(emergency.IsEmergency);
        Assert.True(emergency.IsActive);
        Assert.Equal([7], Assert.Single(notifier.ScreenNotifications));

        IActionResult conflict = await controller.Create(CreateModel("İkinci yayın"), CancellationToken.None);
        Assert.IsType<ViewResult>(conflict);
        Assert.True(controller.ModelState.ContainsKey(nameof(EmergencyFormViewModel.SelectedScreenIds)));
        Assert.Single(await dbContext.Announcements.ToListAsync());

        IActionResult stopped = await controller.Stop(emergency.Id, true, CancellationToken.None);
        Assert.IsType<RedirectToActionResult>(stopped);
        await dbContext.Entry(emergency).ReloadAsync();
        Assert.False(emergency.IsActive);
        Assert.Equal(2, notifier.ScreenNotifications.Count);
        Assert.Equal(2, await dbContext.ActivityLogs.CountAsync());
    }

    private static EmergencyFormViewModel CreateModel(string title) => new()
    {
        Title = title,
        Description = "Lütfen görevlilerin yönlendirmelerine uyun.",
        DurationMinutes = 30,
        SelectedScreenIds = [7],
        IsConfirmed = true
    };

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"emergency-tests-{Guid.NewGuid():N}").Options;
        return new AppDbContext(options);
    }
}
