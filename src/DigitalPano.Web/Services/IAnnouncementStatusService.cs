using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;

namespace DigitalPano.Web.Services;

public interface IAnnouncementStatusService
{
    AnnouncementStatus GetStatus(Announcement announcement);

    string GetDisplayName(AnnouncementStatus status);
}
