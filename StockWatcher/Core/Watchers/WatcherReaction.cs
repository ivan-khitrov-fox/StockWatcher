using StockWatcher.Enums;

namespace StockWatcher.Core.Watchers;

public class WatcherReaction
{
    public ReactionType Type { get; set; }
    public string SecId { get; set; }
    public string Message { get; set; }
}
