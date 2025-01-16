using Newtonsoft.Json;

namespace StockWatcher.Models;

public class RawStockList
{
    [JsonProperty("securities")]
    public RawStockItemSecurity[] Securities { get; set; }
}

public class RawStockItemSecurity
{
    [JsonProperty("SECID")]
    public string SecId { get; set; }

    [JsonProperty("SHORTNAME")]
    public string ShortName { get; set; }

    [JsonProperty("PREVPRICE")]
    public double PrevPrice { get; set; }

    [JsonProperty("LOTSIZE")]
    public int LotSize { get; set; }

    [JsonProperty("LISTLEVEL")]
    public int Level { get; set; }
}
