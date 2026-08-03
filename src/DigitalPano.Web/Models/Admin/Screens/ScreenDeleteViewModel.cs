namespace DigitalPano.Web.Models.Admin.Screens;

public sealed record ScreenDeleteViewModel(
    int Id,
    string Name,
    string Slug,
    int AnnouncementCount);
