namespace DigitalPano.Web.Models.Admin.Announcements;

public sealed record AnnouncementDeleteViewModel(
    int Id,
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    IReadOnlyList<string> ScreenNames);
