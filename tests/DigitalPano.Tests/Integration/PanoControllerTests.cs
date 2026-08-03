using DigitalPano.Web.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Pano;
using DigitalPano.Web.Services;
using DigitalPano.Web.Services.Media;
using DigitalPano.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class PanoControllerTests
{
    [Fact]
    public async Task IndexReturnsOnlyCurrentAnnouncementsAssignedToScreen()
    {
        await using AppDbContext dbContext = CreateContext();
        DateTime utcNow = new(2026, 8, 4, 9, 0, 0, DateTimeKind.Utc);
        AddPanoTestData(dbContext, utcNow);
        await dbContext.SaveChangesAsync();
        PanoController controller = CreateController(dbContext, utcNow);

        IActionResult result = await controller.Index("giris", "correct-key", CancellationToken.None);

        ViewResult view = Assert.IsType<ViewResult>(result);
        PanoViewModel model = Assert.IsType<PanoViewModel>(view.Model);
        PanoContentItemViewModel item = Assert.Single(model.Items);
        Assert.Equal("Aktif duyuru", item.Title);
        Assert.Equal("Giriş", model.ScreenName);
    }

    [Fact]
    public async Task IndexRejectsInvalidDeviceKey()
    {
        await using AppDbContext dbContext = CreateContext();
        DateTime utcNow = new(2026, 8, 4, 9, 0, 0, DateTimeKind.Utc);
        AddPanoTestData(dbContext, utcNow);
        await dbContext.SaveChangesAsync();
        PanoController controller = CreateController(dbContext, utcNow);

        IActionResult result = await controller.Index("giris", "wrong-key", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task HeartbeatUpdatesLastConnectionTime()
    {
        await using AppDbContext dbContext = CreateContext();
        DateTime utcNow = new(2026, 8, 4, 9, 0, 0, DateTimeKind.Utc);
        AddPanoTestData(dbContext, utcNow);
        await dbContext.SaveChangesAsync();
        PanoController controller = CreateController(dbContext, utcNow);

        IActionResult result = await controller.Heartbeat("giris", "correct-key", CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Screen screen = await dbContext.Screens.SingleAsync(x => x.Slug == "giris");
        Assert.Equal(utcNow, screen.LastConnectionDateUtc);
    }

    private static PanoController CreateController(AppDbContext dbContext, DateTime utcNow)
    {
        var controller = new PanoController(
            dbContext,
            new ScreenKeyService(),
            new FakeMediaStorageService(),
            new FixedTimeProvider(new DateTimeOffset(utcNow)))
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        return controller;
    }

    private static void AddPanoTestData(AppDbContext dbContext, DateTime utcNow)
    {
        var targetScreen = new Screen
        {
            Id = 101,
            Name = "Giriş",
            Slug = "giris",
            DeviceKey = "correct-key",
            IsActive = true
        };
        var otherScreen = new Screen
        {
            Id = 102,
            Name = "Diğer",
            Slug = "diger",
            DeviceKey = "other-key",
            IsActive = true
        };
        dbContext.Screens.AddRange(targetScreen, otherScreen);
        dbContext.Announcements.AddRange(
            CreateAnnouncement(201, "Aktif duyuru", utcNow.AddHours(-1), utcNow.AddHours(1), targetScreen),
            CreateAnnouncement(202, "Süresi dolmuş", utcNow.AddHours(-2), utcNow.AddHours(-1), targetScreen),
            CreateAnnouncement(203, "Başka ekran", utcNow.AddHours(-1), utcNow.AddHours(1), otherScreen));
    }

    private static Announcement CreateAnnouncement(
        int id,
        string title,
        DateTime start,
        DateTime end,
        Screen screen)
    {
        return new Announcement
        {
            Id = id,
            Title = title,
            Description = title,
            ContentType = AnnouncementContentType.Text,
            StartDateUtc = start,
            EndDateUtc = end,
            IsActive = true,
            AnnouncementScreens = [new AnnouncementScreen { Screen = screen, ScreenId = screen.Id }]
        };
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"pano-tests-{Guid.NewGuid():N}")
            .Options;
        return new AppDbContext(options);
    }
}

internal sealed class FakeMediaStorageService : IMediaStorageService
{
    public Task<MediaValidationResult> ValidateAsync(IFormFile file, CancellationToken cancellationToken = default) =>
        Task.FromResult(MediaValidationResult.Invalid("Test"));

    public Task<StoredMediaFile> StoreAsync(IFormFile file, MediaValidationResult validation, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Stream?> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream?>(new MemoryStream([1, 2, 3]));

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
