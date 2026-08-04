using DigitalPano.Web.Data;
using DigitalPano.Web.Services.RealTime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Hubs;

[AllowAnonymous]
public sealed class PanoHub(AppDbContext dbContext, TimeProvider timeProvider) : Hub
{
    public override async Task OnConnectedAsync()
    {
        HttpContext? httpContext = Context.GetHttpContext();
        string slug = httpContext?.Request.Query["slug"].ToString() ?? string.Empty;
        string key = httpContext?.Request.Query["key"].ToString() ?? string.Empty;
        Data.Entities.Screen? screen = await dbContext.Screens.SingleOrDefaultAsync(
            x => x.Slug == slug && x.DeviceKey == key && x.IsActive,
            Context.ConnectionAborted);
        if (screen is null)
        {
            Context.Abort();
            return;
        }

        screen.LastConnectionDateUtc = timeProvider.GetUtcNow().UtcDateTime;
        await dbContext.SaveChangesAsync(Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, PanoGroups.ForScreen(screen.Id), Context.ConnectionAborted);
        await Groups.AddToGroupAsync(Context.ConnectionId, PanoGroups.AllScreens, Context.ConnectionAborted);
        await base.OnConnectedAsync();
    }
}
