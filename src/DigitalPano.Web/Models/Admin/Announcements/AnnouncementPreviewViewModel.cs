namespace DigitalPano.Web.Models.Admin.Announcements;

public sealed record AnnouncementPreviewViewModel(
    int Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    int DisplayDurationSeconds,
    AnnouncementStatus Status,
    IReadOnlyList<string> ScreenNames);
