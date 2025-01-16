using Newtonsoft.Json;
using StockWatcher.Models;

namespace StockWatcher.Converters;

public static class RawStockListConverter
{
    public static List<StockItem> ToStockList(string content)
    {
        var rawData = ToRawStockList(content);
        var stockList = ToStockItems(rawData);
        return stockList;
    }

    public static RawStockList ToRawStockList(string content) =>
        JsonConvert.DeserializeObject<RawStockList>(content);

    private static List<StockItem> ToStockItems(RawStockList rawList)
    {
        var items = new List<StockItem>();
        if (rawList.Securities?.Any() != true) return items;

        foreach (var item in rawList.Securities)
        {
            var stockItem = new StockItem
            {
                SecId = item.SecId,
                Name = item.ShortName,
                Level = Convert.ToByte(item.Level),
                LotSize = item.LotSize,
            };
            items.Add(stockItem);
        }
        return items;
    }
}
