using DigitalPano.Web.Controllers;
using DigitalPano.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Tests.Integration;

public sealed class HealthControllerTests
{
    [Fact]
    public void LiveReturnsOkWithoutDatabaseQuery()
    {
        using AppDbContext dbContext = CreateContext();
        var controller = new HealthController(dbContext);

        Assert.IsType<OkObjectResult>(controller.Live());
    }

    [Fact]
    public async Task ReadyReturnsOkWhenDatabaseIsAvailable()
    {
        await using AppDbContext dbContext = CreateContext();
        var controller = new HealthController(dbContext);

        Assert.IsType<OkObjectResult>(await controller.Ready(CancellationToken.None));
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"health-tests-{Guid.NewGuid():N}").Options;
        return new AppDbContext(options);
    }
}
