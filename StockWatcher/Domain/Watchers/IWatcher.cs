using StockWatcher.Enums;
using StockWatcher.Models;

namespace StockWatcher.Domain.Watchers;

public interface IWatcher
{
    public string Id { get; }
    public string SecId { get; }
    public WatcherType Direction { get; }
    public bool Disabled { get; set; }

    Task<WatcherReaction> Analyze(List<HourHistory> data);
}
