using StockWatcher.Enums;
using StockWatcher.Models;

namespace StockWatcher.Domain.Watchers;

/// <summary>
/// Watches absolute (or start-price-relative) price levels for a security.
/// Use this when you want "alert me when price crosses level X/Y" regardless of any specific deal/entry price.
/// </summary>
public class LimitWatcher : WatcherBase
{
    public DateTime DealTime { get; set; }

    // WarningLevel < AlarmLevel when Type = Up
    // WarningLevel > AlarmLevel when Type = Down

    /// <summary>
    /// Absolute price level (currency) for Warning.
    /// If percent mode is configured (<see cref="StartPrice"/> + pct fields), those thresholds take priority.
    /// </summary>
    public double? WarningLevel { get; set; }
    /// <summary>
    /// Absolute price level (currency) for Alarm.
    /// If percent mode is configured (<see cref="StartPrice"/> + pct fields), those thresholds take priority.
    /// </summary>
    public double? AlarmLevel { get; set; }

    /// <summary>
    /// Enables percent mode: thresholds are derived from this start price.
    /// Typical usage: set <see cref="StartPrice"/> once, then configure <see cref="WarningPct"/>/<see cref="AlarmPct"/>.
    /// </summary>
    public double? StartPrice { get; set; }
    /// <summary>
    /// Percent threshold from <see cref="StartPrice"/> for Warning.
    /// </summary>
    public float? WarningPct { get; set; }
    /// <summary>
    /// Percent threshold from <see cref="StartPrice"/> for Alarm.
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
