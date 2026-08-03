using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;

namespace DigitalPano.Web.Services;

public sealed class AnnouncementStatusService(TimeProvider timeProvider) : IAnnouncementStatusService
{
    public AnnouncementStatus GetStatus(Announcement announcement)
    {
        if (!announcement.IsActive)
        {
            return AnnouncementStatus.Inactive;
        }

        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        if (announcement.EndDateUtc < utcNow)
        {
            return AnnouncementStatus.Expired;
        }

        return announcement.StartDateUtc > utcNow
            ? AnnouncementStatus.Scheduled
            : AnnouncementStatus.Active;
    }

    public string GetDisplayName(AnnouncementStatus status) => status switch
    {
        AnnouncementStatus.Active => "Yayında",
        AnnouncementStatus.Scheduled => "Planlanmış",
        AnnouncementStatus.Expired => "Süresi dolmuş",
        AnnouncementStatus.Inactive => "Pasif",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Bilinmeyen duyuru durumu.")
    };
}
