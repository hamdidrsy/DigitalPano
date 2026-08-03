namespace DigitalPano.Web.Models.Admin.Tickers;

public sealed class TickerMessageListViewModel
{
    public IReadOnlyList<TickerMessageListItemViewModel> Items { get; init; } = [];
}

public sealed record TickerMessageListItemViewModel(
    int Id,
    string Text,
    DateTime StartDate,
    DateTime EndDate,
    int SortOrder,
    TickerMessageStatus Status);
