namespace DigitalPano.Web.Data.Entities;

public sealed class TickerMessage
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime StartDateUtc { get; set; }

    public DateTime EndDateUtc { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
