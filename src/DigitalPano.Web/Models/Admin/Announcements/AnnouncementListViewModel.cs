namespace DigitalPano.Web.Models.Admin.Announcements;

using DigitalPano.Web.Data.Entities;

public sealed class AnnouncementListViewModel
{
    public string? Search { get; init; }

    public AnnouncementStatus? Status { get; init; }

    public int? ScreenId { get; init; }

    public IReadOnlyList<AnnouncementListItemViewModel> Items { get; init; } = [];

    public IReadOnlyList<ScreenOptionViewModel> Screens { get; init; } = [];
}

public sealed record AnnouncementListItemViewModel(
    int Id,
    string Title,
    string Description,
    AnnouncementContentType ContentType,
    DateTime StartDate,
    DateTime EndDate,
    int DisplayDurationSeconds,
    int SortOrder,
    AnnouncementStatus Status,
    IReadOnlyList<string> ScreenNames);
