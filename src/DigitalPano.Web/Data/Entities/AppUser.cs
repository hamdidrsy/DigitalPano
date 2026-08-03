using Microsoft.AspNetCore.Identity;

namespace DigitalPano.Web.Data.Entities;

public sealed class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Announcement> CreatedAnnouncements { get; set; } = [];

    public ICollection<ActivityLog> ActivityLogs { get; set; } = [];
}
