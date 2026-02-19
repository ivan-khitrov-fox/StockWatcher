using StockWatcher.Enums;
using StockWatcher.Models;
using StockWatcher.Utilities;

namespace StockWatcher.Domain.Watchers;

/// <summary>
/// Watches price movement relative to a specific user deal (entry price).
/// Use this when you want "profit/loss vs my buy price" signals rather than absolute market levels.
/// </summary>
public class DealWatcher : WatcherBase
{
    public DateTime DealTime { get; set; }
    public double StockItemPrice { get; set; }
    public int StockItemCount { get; set; }

    /// <summary>
    /// Threshold in percent relative to <see cref="StockItemPrice"/>.
    /// For <see cref="WatcherType.Up"/> this is "profit %"; for <see cref="WatcherType.Down"/> it's "drawdown %".
    /// Defaults are intentionally small to be useful without extra configuration.
    /// </summary>
    public float? WarningPct { get; set; }

    /// <summary>
    /// Stronger threshold (same meaning as <see cref="WarningPct"/> but for Alarm). Default 3.
    /// </summary>
    public float? AlarmPct { get; set; }

    public override WatcherReaction Analyze(HourHistory hourHistory)
    {
        if (Disabled || hourHistory == null || StockItemPrice <= 0)
            return new WatcherReaction { Type = ReactionType.Keep };

        var currentPrice = hourHistory.EndPrice;
        var warningPct = WarningPct ?? AppSettings.DealWatcherWarningPctDefault;
        var alarmPct = AlarmPct ?? AppSettings.DealWatcherAlarmPctDefault;

        if (Direction == WatcherType.Up)
        {
            // Watching for profit - price increased above buy price
            var profitPct = (currentPrice - StockItemPrice) / StockItemPrice * 100;
            if (profitPct >= alarmPct)
                return new WatcherReaction { Type = ReactionType.Alarm, Message = $"Deal profit +{profitPct:F1}%. Consider selling.", SecId = hourHistory.SecId };
            if (profitPct >= warningPct)
                return new WatcherReaction { Type = ReactionType.Warning, Message = $"Deal in profit +{profitPct:F1}%.", SecId = hourHistory.SecId };
        }

        if (Direction == WatcherType.Down)
        {
            // Watching for buy opportunity - price dropped below deal price
            var dropPct = (StockItemPrice - currentPrice) / StockItemPrice * 100;
            if (dropPct >= alarmPct)
                return new WatcherReaction { Type = ReactionType.Alarm, Message = $"Price -{dropPct:F1}% below deal. Buy opportunity.", SecId = hourHistory.SecId };
            if (dropPct >= warningPct)
                return new WatcherReaction { Type = ReactionType.Warning, Message = $"Price -{dropPct:F1}% below deal.", SecId = hourHistory.SecId };
        }

        return new WatcherReaction { Type = ReactionType.Keep };
    }
}
