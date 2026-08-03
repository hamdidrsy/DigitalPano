namespace DigitalPano.Web.Models.Admin.Screens;

public sealed class ScreenListViewModel
{
    public IReadOnlyList<ScreenListItemViewModel> Items { get; init; } = [];
}

public sealed record ScreenListItemViewModel(
    int Id,
    string Name,
    string Slug,
    string DeviceKey,
    string? Location,
    bool IsActive,
    bool IsOnline,
    DateTime? LastConnectionDate,
    int AnnouncementCount);
