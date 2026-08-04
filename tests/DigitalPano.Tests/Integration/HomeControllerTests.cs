using DigitalPano.Web.Controllers;
using DigitalPano.Web.Data;
using DigitalPano.Web.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class HomeControllerTests
{
    [Fact]
    public async Task IndexRedirectsToFirstActivePano()
    {
        await using AppDbContext dbContext = CreateContext();
        dbContext.Screens.AddRange(
            new Screen { Id = 1, Name = "Pasif", Slug = "pasif", DeviceKey = "old", IsActive = false },
            new Screen { Id = 2, Name = "Ana pano", Slug = "ana-pano", DeviceKey = "secret", IsActive = true });
        await dbContext.SaveChangesAsync();
        var controller = new HomeController(dbContext);

        RedirectToActionResult result = Assert.IsType<RedirectToActionResult>(
            await controller.Index(CancellationToken.None));

        Assert.Equal("Pano", result.ControllerName);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal("ana-pano", result.RouteValues?["slug"]);
        Assert.Equal("secret", result.RouteValues?["key"]);
    }

    [Fact]
    public async Task IndexShowsSetupMessageWhenNoActiveScreenExists()
    {
        await using AppDbContext dbContext = CreateContext();
        var controller = new HomeController(dbContext);

        Assert.IsType<ViewResult>(await controller.Index(CancellationToken.None));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"home-tests-{Guid.NewGuid():N}").Options;
        return new AppDbContext(options);
    }
}
