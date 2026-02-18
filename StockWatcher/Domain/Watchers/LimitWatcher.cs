using StockWatcher.Enums;
using StockWatcher.Models;

namespace StockWatcher.Domain.Watchers;

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

    public override WatcherReaction Analyze(HourHistory hourHistory)
    {
        if (Disabled || hourHistory == null)
            return new WatcherReaction { Type = ReactionType.Keep };

        double? warning = null;
        double? alarm = null;
        // Percent factor more power than value
        if (StartPrice.HasValue && (WarningPct.HasValue || AlarmPct.HasValue))
        {
            (warning, alarm) = ResolveLevels();
        }
        if (alarm == null)
            alarm = AlarmLevel;
        if (warning == null)
            warning = WarningLevel;

        var price = hourHistory.EndPrice;
        if (Direction == WatcherType.Up)
        {
            if (alarm.HasValue && price > alarm)
            {
                return new WatcherReaction { Type = ReactionType.Alarm, Message = "Up. Sell it.", SecId = hourHistory.SecId };
            }
            if (warning.HasValue && price > warning)
            {
                return new WatcherReaction { Type = ReactionType.Warning, Message = "Up. Warning.", SecId = hourHistory.SecId };
            }
        }

        if (Direction == WatcherType.Down)
        {
            if (alarm.HasValue && price < alarm)
            {
                return new WatcherReaction { Type = ReactionType.Alarm, Message = "Down. Buy it.", SecId = hourHistory.SecId };
            }
            if (warning.HasValue && price < warning)
            {
                return new WatcherReaction { Type = ReactionType.Warning, Message = "Down. Warning.", SecId = hourHistory.SecId };
            }
        }

        return new WatcherReaction { Type = ReactionType.Keep };
    }

    private (double? warning, double? alarm) ResolveLevels()
    {
        // Percent mode has priority if fully configured
        if (StartPrice.HasValue)
        {
            double? warning = null;
            double? alarm = null;

            if (WarningPct.HasValue)
            {
                warning = Direction == WatcherType.Up
                    ? StartPrice.Value * (1 + WarningPct.Value / 100.0)
                    : StartPrice.Value * (1 - WarningPct.Value / 100.0);
            }

            if (AlarmPct.HasValue)
            {
                alarm = Direction == WatcherType.Up
                    ? StartPrice.Value * (1 + AlarmPct.Value / 100.0)
                    : StartPrice.Value * (1 - AlarmPct.Value / 100.0);
            }

            return (warning, alarm);
        }

        // Fallback to absolute levels
        return (WarningLevel, AlarmLevel);
    }
}
