using DigitalPano.Web.Data.Entities;

namespace DigitalPano.Tests.Data;

public sealed class EntityDefaultsTests
{
    [Fact]
    public void NewAnnouncementHasSafeDisplayDefaults()
    {
        var announcement = new Announcement();

        Assert.True(announcement.IsActive);
        Assert.False(announcement.IsEmergency);
        Assert.Equal(10, announcement.DisplayDurationSeconds);
    }

    [Fact]
    public void NewInstitutionUsesProjectThemeAndTimezoneDefaults()
    {
        var institution = new InstitutionSetting();

        Assert.Equal("#0D6EFD", institution.PrimaryColor);
        Assert.Equal("#6C757D", institution.SecondaryColor);
        Assert.Equal("Europe/Istanbul", institution.TimeZoneId);
    }
}
