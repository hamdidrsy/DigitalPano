namespace DigitalPano.Web.Data.Entities;

public sealed class Announcement
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public AnnouncementContentType ContentType { get; set; }

    public DateTime StartDateUtc { get; set; }

    public DateTime EndDateUtc { get; set; }

    public int DisplayDurationSeconds { get; set; } = 10;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsEmergency { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public string? CreatedByUserId { get; set; }

    public int? MediaId { get; set; }

    public AppUser? CreatedByUser { get; set; }

    public Media? Media { get; set; }

    public ICollection<AnnouncementScreen> AnnouncementScreens { get; set; } = [];
}
