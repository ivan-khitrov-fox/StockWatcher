namespace StockWatcher.Core.Watchers;

public class LimitWatcher : WatcherBase
{
    public DateTime DealTime { get; set; }

    // WarningLevel < AlarmLevel when Type = Up
    // WarningLevel > AlarmLevel when Type = Down

    /// <summary>
    ///0 Case when level is set in currency (Warning)
    /// </summary>
    public double? WarningLevel { get; set; }
    /// <summary>
    /// Case when level is set in currency (Alarm)
    /// </summary>
    public double? AlarmLevel { get; set; }

    /// <summary>
    ///  Case when level is set as percent from the start price. (start)
    /// </summary>
    public double? StartPrice { get; set; }
    /// <summary>
    ///  Case when level is set as percent from the start price. (warning)
    /// </summary>
    public float? WarningPct { get; set; }
    /// <summary>
    ///  Case when level is set as percent from the start price. (alarm)
    /// </summary>
    public float? AlarmPct { get; set; }
}
