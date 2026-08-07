using Amazon.S3;
using Amazon.S3.Model;
using DigitalPano.Web.Options;
using Microsoft.Extensions.Options;

namespace DigitalPano.Web.Services.Media;

public sealed class R2MediaStorageService(
    IAmazonS3 s3Client,
    IOptions<R2StorageOptions> r2Options,
    IOptions<MediaStorageOptions> mediaOptions,
    TimeProvider timeProvider) : IMediaStorageService
{
    private readonly R2StorageOptions _r2Options = r2Options.Value;
    private readonly MediaStorageOptions _mediaOptions = mediaOptions.Value;

    public Task<MediaValidationResult> ValidateAsync(
        IFormFile file,
        CancellationToken cancellationToken = default) =>
        MediaFileValidator.ValidateAsync(file, _mediaOptions, cancellationToken);

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

        DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;
        string storedFileName = $"{Guid.NewGuid():N}{validation.Extension}";
        string objectKey = string.Join('/',
            utcNow.Year.ToString(System.Globalization.CultureInfo.InvariantCulture),
            utcNow.Month.ToString("00", System.Globalization.CultureInfo.InvariantCulture),
            storedFileName);

        await using Stream source = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = _r2Options.BucketName,
            Key = objectKey,
            InputStream = source,
            ContentType = validation.MimeType,
            AutoCloseStream = false,
            DisablePayloadSigning = true
        };
        await s3Client.PutObjectAsync(request, cancellationToken);

        return new StoredMediaFile(
            storedFileName,
            objectKey,
            validation.MimeType,
            validation.MediaType.Value,
            file.Length);
    }

    public async Task<Stream?> OpenReadAsync(
        string relativePath,
        CancellationToken cancellationToken = default)
    {
        string objectKey = NormalizeObjectKey(relativePath);
        try
        {
            using GetObjectResponse response = await s3Client.GetObjectAsync(
                _r2Options.BucketName, objectKey, cancellationToken);
            var content = new MemoryStream();
            await response.ResponseStream.CopyToAsync(content, cancellationToken);
            content.Position = 0;
            return content;
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        string objectKey = NormalizeObjectKey(relativePath);
        await s3Client.DeleteObjectAsync(_r2Options.BucketName, objectKey, cancellationToken);
    }

    private static string NormalizeObjectKey(string relativePath)
    {
        string objectKey = relativePath.Replace('\\', '/').TrimStart('/');
        if (string.IsNullOrWhiteSpace(objectKey) || objectKey.Split('/').Any(segment => segment is ".." or "."))
        {
            throw new InvalidOperationException("Geçersiz medya dosyası yolu.");
        }

        return objectKey;
    }
}
