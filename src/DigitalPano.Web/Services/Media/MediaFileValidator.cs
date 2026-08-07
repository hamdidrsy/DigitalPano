using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using Microsoft.AspNetCore.Http;

namespace DigitalPano.Web.Services.Media;

internal static class MediaFileValidator
{
    private const int HeaderLength = 16;

    public static async Task<MediaValidationResult> ValidateAsync(
        IFormFile file,
        MediaStorageOptions options,
        CancellationToken cancellationToken)
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
            ? options.MaxImageBytes
            : options.MaxVideoBytes;
        if (file.Length > maximumSize)
        {
            string limit = detected.MediaType == MediaType.Image
                ? $"{options.MaxImageBytes / 1024 / 1024} MB"
                : $"{options.MaxVideoBytes / 1024 / 1024} MB";
            return MediaValidationResult.Invalid($"Dosya boyutu {limit} sınırını aşamaz.");
        }

        if (!IsAcceptedClientContentType(file.ContentType, detected.MimeType!))
        {
            return MediaValidationResult.Invalid("Dosyanın içerik türü ile gerçek biçimi uyuşmuyor.");
        }

        return detected;
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
