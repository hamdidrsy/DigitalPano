namespace DigitalPano.Web.Data.Entities;

public sealed class Screen
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string DeviceKey { get; set; } = string.Empty;

    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastConnectionDateUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<AnnouncementScreen> AnnouncementScreens { get; set; } = [];
}
