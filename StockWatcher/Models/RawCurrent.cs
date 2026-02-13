using Newtonsoft.Json;

namespace StockWatcher.Models;

public class RawCurrent
{
    [JsonProperty("securities")]
    public RawSecurities Securities { get; set; }
}

public class RawSecurities
{
    [JsonProperty("columns")]
    public string[] Columns { get; set; }

    [JsonProperty("data")]
    public object[][] Data { get; set; }
}
