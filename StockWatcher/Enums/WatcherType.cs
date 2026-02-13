namespace StockWatcher.Enums;

public  enum WatcherType
{
    /// <summary>
    /// Type Up. Watching price increasing (Warning when price exceeds the level)
    /// </summary>
    Up,
    /// <summary>
    /// Type Down. Watching price decreasing (Warning when price fell below the limit)
    /// </summary>
    Down,
    Range
}
