using StockWatcher.Core.Watchers;

namespace StockWatcher.Core;

public interface IAdvisor
{
    public int Id { get; set; }
    public string SecId { get; set; }
    public bool Disabled { get; set; }

    Task<WatcherReaction> Analyze();
}
