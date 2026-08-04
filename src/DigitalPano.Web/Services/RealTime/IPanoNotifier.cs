namespace DigitalPano.Web.Services.RealTime;

public interface IPanoNotifier
{
    Task NotifyScreensAsync(IEnumerable<int> screenIds, CancellationToken cancellationToken = default);
    Task NotifyAllAsync(CancellationToken cancellationToken = default);
}
