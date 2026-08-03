namespace DigitalPano.Web.Data.Entities;

public sealed class Media
{
    public int Id { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string RelativePath { get; set; } = string.Empty;

    public string MimeType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public MediaType MediaType { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<Announcement> Announcements { get; set; } = [];
}
