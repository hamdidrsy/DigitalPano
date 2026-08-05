using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Options;
using DigitalPano.Web.Services.Media;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace DigitalPano.Tests.Services;

public sealed class LocalMediaStorageServiceTests : IDisposable
{
    private readonly string _testRoot = Path.Combine(
        Path.GetTempPath(),
        "DigitalPanoTests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task ValidPngCanBeStoredReadAndDeleted()
    {
        LocalMediaStorageService service = CreateService();
        byte[] content = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1, 2, 3, 4];
        IFormFile file = CreateFormFile("kurum-logo.png", "image/png", content);

        MediaValidationResult validation = await service.ValidateAsync(file);
        StoredMediaFile stored = await service.StoreAsync(file, validation);

        Assert.True(validation.IsValid);
        Assert.Equal(MediaType.Image, stored.MediaType);
        Assert.NotEqual(file.FileName, stored.StoredFileName);
        Assert.EndsWith(".png", stored.StoredFileName, StringComparison.Ordinal);
        await using (Stream? storedStream = await service.OpenReadAsync(stored.RelativePath))
        {
            Assert.NotNull(storedStream);
            using var memory = new MemoryStream();
            await storedStream.CopyToAsync(memory);
            Assert.Equal(content, memory.ToArray());
        }

        await service.DeleteAsync(stored.RelativePath);
        Assert.Null(await service.OpenReadAsync(stored.RelativePath));
    }

    [Theory]
    [MemberData(nameof(SupportedFileHeaders))]
    public async Task SupportedFileSignaturesAreRecognized(
        string fileName,
        string contentType,
        byte[] header,
        MediaType expectedType)
    {
        LocalMediaStorageService service = CreateService();
        IFormFile file = CreateFormFile(fileName, contentType, header);

        MediaValidationResult result = await service.ValidateAsync(file);

        Assert.True(result.IsValid);
        Assert.Equal(expectedType, result.MediaType);
    }

    [Fact]
    public async Task SpoofedPngIsRejected()
    {
        LocalMediaStorageService service = CreateService();
        IFormFile file = CreateFormFile("zararli.png", "image/png", "not-a-png"u8.ToArray());

        MediaValidationResult result = await service.ValidateAsync(file);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("resim.png", "application/octet-stream")]
    [InlineData("resim.png.exe", "image/png")]
    [InlineData("resim.svg", "image/svg+xml")]
    public async Task DangerousExtensionOrMimeCombinationsAreRejected(string fileName, string contentType)
    {
        LocalMediaStorageService service = CreateService();
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1];

        MediaValidationResult result = await service.ValidateAsync(CreateFormFile(fileName, contentType, png));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task EmptyUploadIsRejected()
    {
        LocalMediaStorageService service = CreateService();

        MediaValidationResult result = await service.ValidateAsync(
            CreateFormFile("bos.png", "image/png", []));

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ImageOverConfiguredLimitIsRejected()
    {
        LocalMediaStorageService service = CreateService(maxImageBytes: 8);
        byte[] content = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 1];
        IFormFile file = CreateFormFile("buyuk.png", "image/png", content);

        MediaValidationResult result = await service.ValidateAsync(file);

        Assert.False(result.IsValid);
        Assert.Contains("sınırını", result.ErrorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public async Task PathTraversalIsRejected()
    {
        LocalMediaStorageService service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.OpenReadAsync("../secrets.json"));
    }

    public static IEnumerable<object[]> SupportedFileHeaders()
    {
        yield return ["foto.jpg", "image/jpeg", new byte[] { 0xFF, 0xD8, 0xFF, 1 }, MediaType.Image];
        yield return ["foto.png", "image/png", new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, MediaType.Image];
        yield return ["foto.webp", "image/webp", "RIFF1234WEBP"u8.ToArray(), MediaType.Image];
        yield return ["video.mp4", "video/mp4", new byte[] { 0, 0, 0, 24, 0x66, 0x74, 0x79, 0x70, 1, 2, 3, 4 }, MediaType.Video];
    }

    public void Dispose()
    {
        string allowedParent = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "DigitalPanoTests"));
        string resolvedRoot = Path.GetFullPath(_testRoot);
        if (resolvedRoot.StartsWith(allowedParent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
            Directory.Exists(resolvedRoot))
        {
            Directory.Delete(resolvedRoot, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private LocalMediaStorageService CreateService(long maxImageBytes = 10 * 1024 * 1024)
    {
        Directory.CreateDirectory(_testRoot);
        var environment = new TestWebHostEnvironment { ContentRootPath = _testRoot };
        var options = Options.Create(new MediaStorageOptions
        {
            RootPath = "media",
            MaxImageBytes = maxImageBytes,
            MaxVideoBytes = 200 * 1024 * 1024
        });
        var timeProvider = new FixedTimeProvider(
            new DateTimeOffset(2026, 8, 4, 10, 0, 0, TimeSpan.Zero));
        return new LocalMediaStorageService(environment, options, timeProvider);
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

internal sealed class TestWebHostEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "DigitalPano.Tests";

    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();

    public string WebRootPath { get; set; } = string.Empty;

    public string EnvironmentName { get; set; } = "Testing";

    public string ContentRootPath { get; set; } = string.Empty;

    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
}
