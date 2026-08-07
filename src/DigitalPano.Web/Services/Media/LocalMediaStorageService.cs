using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using Microsoft.Extensions.Options;

namespace DigitalPano.Web.Services.Media;

public sealed class LocalMediaStorageService : IMediaStorageService
{
    private readonly string _rootPath;
    private readonly MediaStorageOptions _options;
    private readonly TimeProvider _timeProvider;

    public LocalMediaStorageService(
        IWebHostEnvironment environment,
        IOptions<MediaStorageOptions> options,
        TimeProvider timeProvider)
    {
        _options = options.Value;
        _timeProvider = timeProvider;
        _rootPath = Path.GetFullPath(Path.Combine(environment.ContentRootPath, _options.RootPath));
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<MediaValidationResult> ValidateAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        return await MediaFileValidator.ValidateAsync(file, _options, cancellationToken);
    }

    public async Task<StoredMediaFile> StoreAsync(
        IFormFile file,
        MediaValidationResult validation,
        CancellationToken cancellationToken = default)
    {
        if (!validation.IsValid || validation.MediaType is null ||
            validation.MimeType is null || validation.Extension is null)
        {
            throw new ArgumentException("Doğrulanmamış dosya saklanamaz.", nameof(validation));
        }

        DateTime utcNow = _timeProvider.GetUtcNow().UtcDateTime;
        string storedFileName = $"{Guid.NewGuid():N}{validation.Extension}";
        string relativeDirectory = Path.Combine(
            utcNow.Year.ToString(System.Globalization.CultureInfo.InvariantCulture),
            utcNow.Month.ToString("00", System.Globalization.CultureInfo.InvariantCulture));
        string relativePath = Path.Combine(relativeDirectory, storedFileName);
        string fullPath = ResolveSafePath(relativePath);
        string? directory = Path.GetDirectoryName(fullPath);
        if (directory is null)
        {
            throw new InvalidOperationException("Medya klasörü oluşturulamadı.");
        }

        Directory.CreateDirectory(directory);
        string temporaryPath = fullPath + ".uploading";
        try
        {
            await using Stream source = file.OpenReadStream();
            await using (var destination = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 81920,
                useAsync: true))
            {
                await source.CopyToAsync(destination, cancellationToken);
            }

            File.Move(temporaryPath, fullPath);
        }
        catch
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }

            throw;
        }

        return new StoredMediaFile(
            storedFileName,
            relativePath.Replace(Path.DirectorySeparatorChar, '/'),
            validation.MimeType,
            validation.MediaType.Value,
            file.Length);
    }

    public Task<Stream?> OpenReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string fullPath = ResolveSafePath(relativePath);
        Stream? stream = File.Exists(fullPath)
            ? new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true)
            : null;
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string fullPath = ResolveSafePath(relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string ResolveSafePath(string relativePath)
    {
        string normalizedRelativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
        string fullPath = Path.GetFullPath(Path.Combine(_rootPath, normalizedRelativePath));
        string requiredPrefix = _rootPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!fullPath.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Geçersiz medya dosyası yolu.");
        }

        return fullPath;
    }

}
