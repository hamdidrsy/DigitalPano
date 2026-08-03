using Microsoft.AspNetCore.Http;

namespace DigitalPano.Web.Services.Media;

public interface IMediaStorageService
{
    Task<MediaValidationResult> ValidateAsync(
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task<StoredMediaFile> StoreAsync(
        IFormFile file,
        MediaValidationResult validation,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
}
