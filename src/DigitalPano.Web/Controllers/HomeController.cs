using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DigitalPano.Web.Models;
using DigitalPano.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Controllers;

public class HomeController(AppDbContext dbContext) : Controller
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var screen = await dbContext.Screens.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Id)
            .Select(x => new { x.Slug, x.DeviceKey })
            .FirstOrDefaultAsync(cancellationToken);

        return screen is null
            ? View()
            : RedirectToAction("Index", "Pano", new { slug = screen.Slug, key = screen.DeviceKey });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
