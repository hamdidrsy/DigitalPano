using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Web.Models.Pano;

public sealed class PanoViewModel
{
    public string InstitutionName { get; init; } = string.Empty;

    public string? LogoPath { get; init; }

    public int? LogoMediaId { get; init; }

    public string PrimaryColor { get; init; } = "#0D6EFD";

    public string SecondaryColor { get; init; } = "#6C757D";

    public string ScreenName { get; init; } = string.Empty;

    public string ScreenSlug { get; init; } = string.Empty;

    public string DeviceKey { get; init; } = string.Empty;

    public IReadOnlyList<PanoContentItemViewModel> Items { get; init; } = [];

    public IReadOnlyList<string> TickerMessages { get; init; } = [];
}

public sealed record PanoContentItemViewModel(
    int Id,
    string Title,
    string Description,
    AnnouncementContentType ContentType,
    int? MediaId,
    string? MediaMimeType,
    int DisplayDurationSeconds);
