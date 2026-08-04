namespace DigitalPano.Web.Models.Admin.Emergencies;

public sealed class EmergencyListViewModel
{
    public IReadOnlyList<EmergencyListItemViewModel> Items { get; init; } = [];
}

public sealed record EmergencyListItemViewModel(
    int Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsLive,
    IReadOnlyList<string> ScreenNames);
