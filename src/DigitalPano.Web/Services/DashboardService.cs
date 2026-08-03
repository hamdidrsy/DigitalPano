using DigitalPano.Web.Data;
using DigitalPano.Web.Models.Admin;
using Microsoft.EntityFrameworkCore;

namespace DigitalPano.Web.Services;

public sealed class DashboardService(AppDbContext dbContext, TimeProvider timeProvider) : IDashboardService
{
    public async Task<DashboardViewModel> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        DateTime onlineThreshold = utcNow.AddMinutes(-2);

        int activeAnnouncementCount = await dbContext.Announcements.CountAsync(
            x => x.IsActive && x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow,
            cancellationToken);
        int scheduledAnnouncementCount = await dbContext.Announcements.CountAsync(
            x => x.IsActive && x.StartDateUtc > utcNow,
            cancellationToken);
        int expiredAnnouncementCount = await dbContext.Announcements.CountAsync(
            x => x.EndDateUtc < utcNow,
            cancellationToken);
        int activeScreenCount = await dbContext.Screens.CountAsync(x => x.IsActive, cancellationToken);
        int onlineScreenCount = await dbContext.Screens.CountAsync(
            x => x.IsActive && x.LastConnectionDateUtc >= onlineThreshold,
            cancellationToken);
        int emergencyAnnouncementCount = await dbContext.Announcements.CountAsync(
            x => x.IsActive && x.IsEmergency && x.StartDateUtc <= utcNow && x.EndDateUtc >= utcNow,
            cancellationToken);

        return new DashboardViewModel(
            activeAnnouncementCount,
            scheduledAnnouncementCount,
            expiredAnnouncementCount,
            activeScreenCount,
            onlineScreenCount,
            emergencyAnnouncementCount);
    }
}
