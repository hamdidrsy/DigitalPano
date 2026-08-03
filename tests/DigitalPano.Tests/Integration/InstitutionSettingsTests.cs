using DigitalPano.Web.Areas.Admin.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Settings;
using DigitalPano.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class InstitutionSettingsTests
{
    [Fact]
    public async Task UpdatePersistsInstitutionThemeAndImageLogo()
    {
        await using AppDbContext dbContext = CreateContext();
        dbContext.Media.Add(new Media
        {
            Id = 50,
            OriginalFileName = "logo.png",
            StoredFileName = "logo-stored.png",
            RelativePath = "2026/08/logo-stored.png",
            MimeType = "image/png",
            FileSize = 100,
            MediaType = MediaType.Image
        });
        await dbContext.SaveChangesAsync();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.Zero));
        var httpContext = new DefaultHttpContext();
        var controller = new SettingsController(dbContext, timeProvider)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };
        var model = new InstitutionSettingsViewModel
        {
            InstitutionName = "Özel Eğitim Merkezi",
            LogoMediaId = 50,
            PrimaryColor = "#123abc",
            SecondaryColor = "#fedcba",
            City = "İstanbul"
        };

        IActionResult result = await controller.Index(model, CancellationToken.None);

        Assert.IsType<RedirectToActionResult>(result);
        InstitutionSetting settings = await dbContext.InstitutionSettings.SingleAsync();
        Assert.Equal("Özel Eğitim Merkezi", settings.InstitutionName);
        Assert.Equal("2026/08/logo-stored.png", settings.LogoPath);
        Assert.Equal("#123ABC", settings.PrimaryColor);
        Assert.Equal("#FEDCBA", settings.SecondaryColor);
        Assert.Single(await dbContext.ActivityLogs.ToListAsync());
    }

    [Fact]
    public async Task UpdateRejectsVideoAsInstitutionLogo()
    {
        await using AppDbContext dbContext = CreateContext();
        dbContext.Media.Add(new Media
        {
            Id = 60,
            OriginalFileName = "video.mp4",
            StoredFileName = "video-stored.mp4",
            RelativePath = "2026/08/video-stored.mp4",
            MimeType = "video/mp4",
            FileSize = 100,
            MediaType = MediaType.Video
        });
        await dbContext.SaveChangesAsync();
        var timeProvider = new FixedTimeProvider(new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.Zero));
        var httpContext = new DefaultHttpContext();
        var controller = new SettingsController(dbContext, timeProvider)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, new TestTempDataProvider())
        };

        IActionResult result = await controller.Index(new InstitutionSettingsViewModel
        {
            InstitutionName = "Kurum",
            LogoMediaId = 60,
            PrimaryColor = "#123ABC",
            SecondaryColor = "#FEDCBA",
            City = "İstanbul"
        }, CancellationToken.None);

        Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.Empty(await dbContext.InstitutionSettings.ToListAsync());
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"institution-tests-{Guid.NewGuid():N}")
            .Options;
        return new AppDbContext(options);
    }
}
