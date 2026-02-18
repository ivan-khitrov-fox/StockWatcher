using StockWatcher.Enums;
using StockWatcher.Models;

namespace StockWatcher.Domain.Watchers;

public class WatcherBase : IWatcher
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string SecId { get; set; }
    public WatcherType Direction { get; set; }
    public bool Disabled { get; set; } = false;

    public virtual WatcherReaction Analyze(HourHistory hourHistory)
    {
        if (hourHistory == null)
            return new WatcherReaction { Type = ReactionType.Keep };
        //var trend = TrendCalculationHelper.CalculateTrendByHoursHistory(history);
        //return new WatcherReaction { Type = ReactionType.Keep };
        return new WatcherReaction { Type = ReactionType.Keep };
    }

    protected readonly HashSet<string> _triggered = new();
}
