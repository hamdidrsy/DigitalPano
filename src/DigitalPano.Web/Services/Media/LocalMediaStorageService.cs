using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using Microsoft.Extensions.Options;

namespace DigitalPano.Web.Services.Media;

public sealed class LocalMediaStorageService : IMediaStorageService
{
    private const int HeaderLength = 16;
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
        if (file.Length <= 0)
        {
            return MediaValidationResult.Invalid("Boş dosya yüklenemez.");
        }

        string originalFileName = Path.GetFileName(file.FileName);
        if (string.IsNullOrWhiteSpace(originalFileName) || originalFileName.Length > 255)
        {
            return MediaValidationResult.Invalid("Dosya adı geçersiz veya çok uzundur.");
        }

        string extension = Path.GetExtension(originalFileName).ToLowerInvariant();
        await using Stream stream = file.OpenReadStream();
        byte[] header = new byte[HeaderLength];
        int bytesRead = await stream.ReadAsync(header.AsMemory(0, HeaderLength), cancellationToken);

        MediaValidationResult detected = DetectFileType(header.AsSpan(0, bytesRead), extension);
        if (!detected.IsValid || detected.MediaType is null)
        {
            return detected;
        }

        long maximumSize = detected.MediaType == MediaType.Image
            ? _options.MaxImageBytes
            : _options.MaxVideoBytes;
        if (file.Length > maximumSize)
        {
            string limit = detected.MediaType == MediaType.Image
                ? $"{_options.MaxImageBytes / 1024 / 1024} MB"
                : $"{_options.MaxVideoBytes / 1024 / 1024} MB";
            return MediaValidationResult.Invalid($"Dosya boyutu {limit} sınırını aşamaz.");
        }

        if (!IsAcceptedClientContentType(file.ContentType, detected.MimeType!))
        {
            return MediaValidationResult.Invalid("Dosyanın içerik türü ile gerçek biçimi uyuşmuyor.");
        }

        return detected;
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

    private static MediaValidationResult DetectFileType(ReadOnlySpan<byte> header, string extension)
    {
        if ((extension is ".jpg" or ".jpeg") &&
            header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
        {
            return MediaValidationResult.Valid(MediaType.Image, "image/jpeg", extension);
        }

        ReadOnlySpan<byte> pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        if (extension == ".png" && header.StartsWith(pngSignature))
        {
            return MediaValidationResult.Valid(MediaType.Image, "image/png", extension);
        }

        if (extension == ".webp" && header.Length >= 12 &&
            header[..4].SequenceEqual("RIFF"u8) && header.Slice(8, 4).SequenceEqual("WEBP"u8))
        {
            return MediaValidationResult.Valid(MediaType.Image, "image/webp", extension);
        }

        if (extension == ".mp4" && header.Length >= 12 && header.Slice(4, 4).SequenceEqual("ftyp"u8))
        {
            return MediaValidationResult.Valid(MediaType.Video, "video/mp4", extension);
        }

        return MediaValidationResult.Invalid("Yalnızca geçerli JPEG, PNG, WebP veya MP4 dosyaları yüklenebilir.");
    }

    private static bool IsAcceptedClientContentType(string clientContentType, string detectedMimeType)
    {
        return string.Equals(clientContentType, detectedMimeType, StringComparison.OrdinalIgnoreCase) ||
               detectedMimeType == "image/jpeg" &&
               string.Equals(clientContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
               detectedMimeType == "video/mp4" &&
               string.Equals(clientContentType, "application/mp4", StringComparison.OrdinalIgnoreCase);
    }
}
