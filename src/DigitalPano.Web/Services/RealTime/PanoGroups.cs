using System.Globalization;

namespace DigitalPano.Web.Services.RealTime;

public static class PanoGroups
{
    public const string AllScreens = "pano:all";
    public static string ForScreen(int screenId) => $"pano:{screenId.ToString(CultureInfo.InvariantCulture)}";
}
