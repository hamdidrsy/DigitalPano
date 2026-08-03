using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Web.Services.Media;

public sealed record MediaValidationResult(
    bool IsValid,
    string? ErrorMessage,
    MediaType? MediaType,
    string? MimeType,
    string? Extension)
{
    public static MediaValidationResult Invalid(string errorMessage) =>
        new(false, errorMessage, null, null, null);

    public static MediaValidationResult Valid(
        MediaType mediaType,
        string mimeType,
        string extension) =>
        new(true, null, mediaType, mimeType, extension);
}
