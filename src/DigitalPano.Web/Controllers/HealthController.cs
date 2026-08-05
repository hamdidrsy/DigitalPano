using DigitalPano.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Controllers;

[ApiController]
[AllowAnonymous]
public sealed class HealthController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("health/live")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Live() => Ok(new { status = "ok" });

    [HttpGet("health/ready")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Ready(CancellationToken cancellationToken)
    {
        bool databaseAvailable = await dbContext.Database.CanConnectAsync(cancellationToken);
        return databaseAvailable
            ? Ok(new { status = "ready", database = "ok" })
            : StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { status = "not-ready", database = "unavailable" });
    }
}
