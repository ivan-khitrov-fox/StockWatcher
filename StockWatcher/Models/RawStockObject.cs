using Newtonsoft.Json;

namespace StockWatcher.Models;

public class RawStockObject
{
    [JsonProperty("securities")]
    public RawStockItemSecurity Securities { get; set; }
}

public class RawStockItemSecurity
{
    [JsonProperty("columns")]
    public string[] Columns { get; set; }

    [JsonProperty("data")]
    public object[][] Data { get; set; }
}
