using Newtonsoft.Json;
using StockWatcher.Models;

namespace StockWatcher.Converters;

public static class RawStockListConverter
{
    public static List<StockItem> ToStockList(string content)
    {
        var stockList = ToStockItems(content);
        return stockList;
    }

    public static List<StockItem> ToStockItems(string content)
    {
        var items = new List<StockItem>();
        var rawObject = JsonConvert.DeserializeObject<RawStockObject>(content);

        if (rawObject.Securities.Columns?.Any() != true) return items;
        if (rawObject.Securities.Data?.Any() != true) return items;

        var secIndex = Array.IndexOf(rawObject.Securities.Columns, "SECID");
        var shortNameIndex = Array.IndexOf(rawObject.Securities.Columns, "SHORTNAME");
        var priceIndex = Array.IndexOf(rawObject.Securities.Columns, "PREVPRICE");
        var lotSizeIndex = Array.IndexOf(rawObject.Securities.Columns, "LOTSIZE");
        var levelIndex = Array.IndexOf(rawObject.Securities.Columns, "LISTLEVEL");

        if (secIndex == -1 || shortNameIndex == -1 || priceIndex == -1 || lotSizeIndex == -1 || levelIndex == -1)
        {
            return items;
        }

        foreach (var data in rawObject.Securities.Data)
        {
            var stockItem = new StockItem
            {
                SecId = (string)data[secIndex],
                Name = (string)data[shortNameIndex],
                Level = Convert.ToByte(data[levelIndex]),
                LotSize = Convert.ToInt32(data[lotSizeIndex])
            };
            items.Add(stockItem);
        }
        return items;
    }
}
