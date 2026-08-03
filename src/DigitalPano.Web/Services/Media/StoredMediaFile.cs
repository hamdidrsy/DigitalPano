using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Web.Services.Media;

public sealed record StoredMediaFile(
    string StoredFileName,
    string RelativePath,
    string MimeType,
    MediaType MediaType,
    long FileSize);
