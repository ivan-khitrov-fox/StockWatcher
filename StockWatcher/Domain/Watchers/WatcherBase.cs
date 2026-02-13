using StockWatcher.Enums;
using StockWatcher.Models;
using StockWatcher.Utilities;

namespace StockWatcher.Domain.Watchers;

public class WatcherBase : IWatcher
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string SecId { get; set; }
    public WatcherType Direction { get; set; }
    public bool Disabled { get; set; } = false;

    public async Task<WatcherReaction> Analyze(List<HourHistory> history)
    {
        var trend = TrendCalculationHelper.CalculateTrendByHoursHistory(history);
        return new WatcherReaction { Type = ReactionType.Keep };
    }
}
