using Newtonsoft.Json;
using StockWatcher.Models;

namespace StockWatcher.Converters;

public static class RawCurrentPriceDataConverter
{
    public static CurrentPrice ToCurrentPriceData(string content)
    {
        var rawData = ToRawCurrent(content);
        var currentPrice = ToCurrentPrice(rawData);
        return currentPrice;
    }

    public static RawCurrent ToRawCurrent(string content) =>
        JsonConvert.DeserializeObject<RawCurrent>(content);

    private static CurrentPrice ToCurrentPrice(RawCurrent rawCurrent)
    {
        var secIdIndex = Array.IndexOf(rawCurrent.Securities.Columns, "SECID");
        var priceIndex = Array.IndexOf(rawCurrent.Securities.Columns, "PREVPRICE");
        if (secIdIndex == -1 || priceIndex == -1) return null;
        if (rawCurrent.Securities.Data?.Any() != true) return null;

        var model = new CurrentPrice
        {
            SecId = (string)rawCurrent.Securities.Data.First()[secIdIndex],
            Price = Convert.ToDouble(rawCurrent.Securities.Data.First()[priceIndex]),
            Time = DateTime.Now
        };
        return model;
    }
}
