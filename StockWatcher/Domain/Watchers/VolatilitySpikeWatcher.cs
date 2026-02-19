using StockWatcher.Enums;
using StockWatcher.Models;

namespace StockWatcher.Domain.Watchers;

/// <summary>
/// Detects abrupt price change relative to recent moving average.
/// Keeps internal sliding window of recent hours.
/// </summary>
public class VolatilitySpikeWatcher : WatcherBase
{
    private readonly Queue<(DateTime time, double price)> _window = new();
    private double _sum;

    /// <summary>
    /// Percent deviation required to trigger.
    /// </summary>
    public float PercentThreshold { get; set; } = 4;

    /// <summary>
    /// Size of averaging window in hours.
    /// </summary>
    public int WindowHours { get; set; } = 3;

    public override WatcherReaction Analyze(HourHistory hourHistory)
    {
        if (Disabled || hourHistory == null)
            return new WatcherReaction { Type = ReactionType.Keep };

        var now = hourHistory.EndTime;
        var price = hourHistory.EndPrice;

        if (price <= 0)
            return new WatcherReaction { Type = ReactionType.Keep };

        // --- add new sample ---
        _window.Enqueue((now, price));
        _sum += price;

        // --- trim old samples ---
        var border = now.AddHours(-WindowHours);
        while (_window.Count > 0 && _window.Peek().time < border)
        {
            var old = _window.Dequeue();
            _sum -= old.price;
        }

        // need enough data
        if (_window.Count < 3)
            return new WatcherReaction { Type = ReactionType.Keep };

        var avg = _sum / _window.Count;
        if (avg <= 0)
            return new WatcherReaction { Type = ReactionType.Keep };

        var deltaPct = (price - avg) / avg * 100.0;

        // --- UP ---
        if (Direction == WatcherType.Up && deltaPct >= PercentThreshold)
        {
            return new WatcherReaction
            {
                Type = ReactionType.Alarm,
                Message = $"Spike UP {deltaPct:F1}%",
                SecId = hourHistory.SecId
            };
        }

        // --- DOWN ---
        if (Direction == WatcherType.Down && deltaPct <= -PercentThreshold)
        {
            return new WatcherReaction
            {
                Type = ReactionType.Alarm,
                Message = $"Spike DOWN {deltaPct:F1}%",
                SecId = hourHistory.SecId
            };
        }

        return new WatcherReaction { Type = ReactionType.Keep };
    }
}
