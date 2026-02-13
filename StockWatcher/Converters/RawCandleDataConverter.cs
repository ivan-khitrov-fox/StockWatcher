using Newtonsoft.Json;
using StockWatcher.Models;

namespace StockWatcher.Converters;

public static class RawCandleDataConverter
{
    public static List<HourHistory> ToCandleData(string secId, int minutesInterval, string content)
    {
        var rawData = ToRawCandle(content);
        var history = ToCandleData(secId, minutesInterval, rawData);
        return history;
    }

    public static RawHourHistory ToRawCandle(string content) =>
        JsonConvert.DeserializeObject<RawHourHistory>(content);

    private static List<HourHistory> ToCandleData(string secId, int minutesInterval, RawHourHistory rawHourHistory)
    {
        var history = new List<HourHistory>();
        if (!rawHourHistory.Candles.Any()) return history;
        var rawCandle = rawHourHistory.Candles.First();
        if (rawCandle.Data?.Any() != true) return history;

        foreach (var data in rawCandle.Data)
        {
            if (data.Count() < 5) continue;
            if (!(data[0] is long)) continue;

            var unixTimeSeconds = (long)data[0];
            var time = DateTimeOffset.FromUnixTimeMilliseconds(unixTimeSeconds).DateTime;
            // Important! Working in local (Moscow) timezone
            time = DateTime.SpecifyKind(time, DateTimeKind.Local);
            var hourHistoryItem = new HourHistory
            {
                SecId = secId,
                StartTime = time,
                EndTime = time.AddMinutes(minutesInterval),
                StartPrice = Convert.ToDouble(data[1]),
                EndPrice = Convert.ToDouble(data[4])
            };
            history.Add(hourHistoryItem);
        }
        return history;
    }
}
