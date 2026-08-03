namespace DigitalPano.Web.Options;

public sealed class MediaStorageOptions
{
    public const string SectionName = "MediaStorage";

    public string RootPath { get; set; } = "App_Data/media";

    public long MaxImageBytes { get; set; } = 10 * 1024 * 1024;

    public long MaxVideoBytes { get; set; } = 200 * 1024 * 1024;
}
