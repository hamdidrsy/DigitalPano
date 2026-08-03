namespace DigitalPano.Web.Models.Admin.Announcements;

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
    DateTime StartDate,
    DateTime EndDate,
    int DisplayDurationSeconds,
    int SortOrder,
    AnnouncementStatus Status,
    IReadOnlyList<string> ScreenNames);
