using DigitalPano.Web.Models.Admin;

namespace DigitalPano.Web.Services;

public interface IDashboardService
{
    Task<DashboardViewModel> GetSummaryAsync(CancellationToken cancellationToken = default);
}
