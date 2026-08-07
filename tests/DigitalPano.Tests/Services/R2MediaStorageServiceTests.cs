using Amazon.S3;
using Amazon.S3.Model;
using DigitalPano.Tests.Services;
using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using DigitalPano.Web.Services.Media;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace DigitalPano.Tests.Services;

public sealed class R2MediaStorageServiceTests
{
    [Fact]
    public async Task ValidImageIsUploadedWithPrivateObjectKey()
    {
        var s3 = new Mock<IAmazonS3>();
        PutObjectRequest? captured = null;
        s3.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PutObjectRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new PutObjectResponse());
        R2MediaStorageService service = CreateService(s3.Object);
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1];
        IFormFile file = CreateFormFile("duyuru.png", "image/png", png);

        MediaValidationResult validation = await service.ValidateAsync(file);
        StoredMediaFile stored = await service.StoreAsync(file, validation);

        Assert.True(validation.IsValid);
        Assert.Equal(MediaType.Image, stored.MediaType);
        Assert.Matches(@"^2026/08/[a-f0-9]{32}\.png$", stored.RelativePath);
        Assert.NotNull(captured);
        Assert.Equal("digitalpano-media", captured.BucketName);
        Assert.Equal(stored.RelativePath, captured.Key);
        Assert.Equal("image/png", captured.ContentType);
    }

    [Fact]
    public async Task StoredObjectCanBeReadAndDeleted()
    {
        byte[] content = [1, 2, 3, 4];
        var s3 = new Mock<IAmazonS3>();
        s3.Setup(x => x.GetObjectAsync("digitalpano-media", "2026/08/test.png", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetObjectResponse { ResponseStream = new MemoryStream(content) });
        s3.Setup(x => x.DeleteObjectAsync("digitalpano-media", "2026/08/test.png", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeleteObjectResponse());
        R2MediaStorageService service = CreateService(s3.Object);

        await using Stream? stream = await service.OpenReadAsync("2026/08/test.png");
        Assert.NotNull(stream);
        using var copy = new MemoryStream();
        await stream.CopyToAsync(copy);
        Assert.Equal(content, copy.ToArray());

        await service.DeleteAsync("2026/08/test.png");
        s3.Verify(x => x.DeleteObjectAsync(
            "digitalpano-media", "2026/08/test.png", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TraversalObjectKeyIsRejected()
    {
        R2MediaStorageService service = CreateService(Mock.Of<IAmazonS3>());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.OpenReadAsync("../secret.json"));
    }

    private static R2MediaStorageService CreateService(IAmazonS3 s3Client)
    {
        return new R2MediaStorageService(
            s3Client,
            Options.Create(new R2StorageOptions
            {
                Endpoint = "https://account.r2.cloudflarestorage.com",
                AccessKeyId = "access-key",
                SecretAccessKey = "secret-key",
                BucketName = "digitalpano-media"
            }),
            Options.Create(new MediaStorageOptions
            {
                Provider = "R2",
                MaxImageBytes = 10 * 1024 * 1024,
                MaxVideoBytes = 200 * 1024 * 1024
            }),
            new FixedTimeProvider(new DateTimeOffset(2026, 8, 7, 10, 0, 0, TimeSpan.Zero)));
    }

    private static FormFile CreateFormFile(string fileName, string contentType, byte[] content)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "File", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
