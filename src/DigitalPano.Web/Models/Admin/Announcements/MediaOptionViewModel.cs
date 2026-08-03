using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Web.Models.Admin.Announcements;

public sealed record MediaOptionViewModel(
    int Id,
    string OriginalFileName,
    MediaType MediaType);
