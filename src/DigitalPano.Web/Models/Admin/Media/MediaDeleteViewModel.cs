using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Web.Models.Admin.Media;

public sealed record MediaDeleteViewModel(
    int Id,
    string OriginalFileName,
    MediaType MediaType,
    long FileSize,
    int UsageCount);
