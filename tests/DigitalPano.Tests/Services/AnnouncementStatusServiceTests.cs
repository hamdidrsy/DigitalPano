using DigitalPano.Web.Data.Entities;
using DigitalPano.Web.Models.Admin.Announcements;
using DigitalPano.Web.Services;

namespace DigitalPano.Tests.Services;

public sealed class AnnouncementStatusServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 4, 9, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(false, -1, 1, AnnouncementStatus.Inactive)]
    [InlineData(true, 1, 2, AnnouncementStatus.Scheduled)]
    [InlineData(true, -2, -1, AnnouncementStatus.Expired)]
    [InlineData(true, -1, 1, AnnouncementStatus.Active)]
    public void GetStatusReturnsExpectedStatus(
        bool isActive,
        int startHourOffset,
        int endHourOffset,
        AnnouncementStatus expected)
    {
        var service = new AnnouncementStatusService(new FixedTimeProvider(Now));
        var announcement = new Announcement
        {
            IsActive = isActive,
            StartDateUtc = Now.UtcDateTime.AddHours(startHourOffset),
            EndDateUtc = Now.UtcDateTime.AddHours(endHourOffset)
        };

        AnnouncementStatus actual = service.GetStatus(announcement);

        Assert.Equal(expected, actual);
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
