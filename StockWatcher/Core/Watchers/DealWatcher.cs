namespace StockWatcher.Core.Watchers;

public class DealWatcher : WatcherBase
{
    public DateTime DealTime { get; set; }
    public double StockItemPrice { get; set; }
    public int StockItemCount { get; set; }
}
