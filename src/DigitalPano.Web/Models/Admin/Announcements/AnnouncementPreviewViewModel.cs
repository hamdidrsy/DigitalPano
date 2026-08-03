namespace DigitalPano.Web.Models.Admin.Announcements;

using DigitalPano.Web.Data.Entities;

public sealed record AnnouncementPreviewViewModel(
    int Id,
    string Title,
    string Description,
    AnnouncementContentType ContentType,
    int? MediaId,
    string? MediaMimeType,
    DateTime StartDate,
    DateTime EndDate,
    int DisplayDurationSeconds,
    AnnouncementStatus Status,
    IReadOnlyList<string> ScreenNames);
