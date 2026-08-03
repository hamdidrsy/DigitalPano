namespace DigitalPano.Web.Services;

public interface IInstitutionDateTimeService
{
    DateTime ToLocalTime(DateTime utcDateTime);

    DateTime ToUtc(DateTime localDateTime);
}
