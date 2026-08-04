using DigitalPano.Web.Data;
using DigitalPano.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Services.RealTime;

public sealed class SignalRPanoNotifier(IHubContext<PanoHub> hubContext, AppDbContext dbContext) : IPanoNotifier
{
    public async Task NotifyScreensAsync(IEnumerable<int> screenIds, CancellationToken cancellationToken = default)
    {
        string[] groups = screenIds.Distinct().Select(PanoGroups.ForScreen).ToArray();
        if (groups.Length > 0)
        {
            await hubContext.Clients.Groups(groups).SendAsync("YayinDegisti", cancellationToken);
        }
    }

    public async Task NotifyAllAsync(CancellationToken cancellationToken = default)
    {
        int[] screenIds = await dbContext.Screens.AsNoTracking().Where(x => x.IsActive)
            .Select(x => x.Id).ToArrayAsync(cancellationToken);
        await NotifyScreensAsync(screenIds, cancellationToken);
    }
}
