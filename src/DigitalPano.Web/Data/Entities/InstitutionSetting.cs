namespace DigitalPano.Web.Data.Entities;

public sealed class InstitutionSetting
{
    public int Id { get; set; }

    public string InstitutionName { get; set; } = string.Empty;

    public string? LogoPath { get; set; }

    public string PrimaryColor { get; set; } = "#0D6EFD";

    public string SecondaryColor { get; set; } = "#6C757D";

    public string City { get; set; } = "İstanbul";

    public string TimeZoneId { get; set; } = "Europe/Istanbul";

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
