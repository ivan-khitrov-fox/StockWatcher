using Newtonsoft.Json;

namespace StockWatcher.Models;

public class RawHourHistory
{
    [JsonProperty("candles")]
    public RawCandle[] Candles { get; set; }
}

public class RawCandle
{
    [JsonProperty("data")]
    public object[][] Data { get; set; }
}
