namespace DigitalPano.Web.Services;

public sealed class InstitutionDateTimeService : IInstitutionDateTimeService
{
    private static readonly TimeZoneInfo InstitutionTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

    public DateTime ToLocalTime(DateTime utcDateTime)
    {
        DateTime normalizedUtc = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(normalizedUtc, InstitutionTimeZone);
    }

    public DateTime ToUtc(DateTime localDateTime)
    {
        DateTime unspecifiedLocal = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(unspecifiedLocal, InstitutionTimeZone);
    }
}
