using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Web.Models.Admin.Media;

public sealed class MediaListViewModel
{
    public IReadOnlyList<MediaListItemViewModel> Items { get; init; } = [];
}

public sealed record MediaListItemViewModel(
    int Id,
    string OriginalFileName,
    string MimeType,
    long FileSize,
    MediaType MediaType,
    DateTime CreatedAt,
    int UsageCount);
