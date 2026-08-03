namespace DigitalPano.Web.Data.Entities;

public sealed class ActivityLog
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    public string ActionType { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public string? EntityId { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? IpAddress { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AppUser? User { get; set; }
}
