namespace DigitalPano.Web.Data.Entities;

public sealed class AnnouncementScreen
{
    public int AnnouncementId { get; set; }

    public int ScreenId { get; set; }

    public Announcement Announcement { get; set; } = null!;

    public Screen Screen { get; set; } = null!;
}
