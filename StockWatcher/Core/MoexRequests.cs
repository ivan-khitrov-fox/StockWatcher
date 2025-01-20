using System.Net;
using System.Net.Http.Headers;
using StockWatcher.Converters;
using StockWatcher.Models;

namespace StockWatcher.Core;

public class MoexRequests
{
    private readonly Uri baseMoexUrl = new Uri("https://iss.moex.com");
    private int _boardNumber = 57;
    private int _historyIntervalInMinutes = 10;

    public async Task<List<StockItem>> GetStoclList()
    {
        var uri = BuildListUrl(_boardNumber);
        var httpClient = GetHttpClient();
        var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.StatusCode.ToString());
        }
        string responseStringContent = await response.Content.ReadAsStringAsync();
        var stockList = RawStockListConverter.ToStockList(responseStringContent);
        return stockList;
    }

    public async Task<List<HourHistory>> GetLastHourHistory(string secId)
    {
        var minutesInterval = _historyIntervalInMinutes;
        var uri = BuildHistoryUrl(_boardNumber, secId, interval: minutesInterval);
        var httpClient = GetHttpClient();
        var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.StatusCode.ToString());
        }
        string responseStringContent = await response.Content.ReadAsStringAsync();
        var history = RawCandleDataConverter.ToCandleData(secId, minutesInterval, responseStringContent);
        return history;
    }

    public async Task<CurrentPrice> GetCurrent(string secId)
    {
        var uri = BuildCurrentUrl(secId);
        var httpClient = GetHttpClient();
        var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(response.StatusCode.ToString());
        }
        string responseStringContent = await response.Content.ReadAsStringAsync();
        var currentPrice = RawCurrentPriceDataConverter.ToCurrentPriceData(responseStringContent);
        return currentPrice;
    }

    private HttpClient GetHttpClient()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
        var client = new HttpClient(handler);
        client.Timeout = new TimeSpan(0, 0, 10);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
        client.MaxResponseContentBufferSize = 1024000000;
        return client;
    }

    private Uri BuildListUrl(int boardNumber)
    {
        var path = $"iss/engines/stock/markets/shares/boardgroups/{boardNumber}/securities.json?iss.meta=off&lang=ru&security_collection=3&sort_column=SHORTNAME&sort_order=asc";
        var uri = new Uri(baseMoexUrl, path, true);
        return uri;
    }

    /// <summary>
    /// Creates last hour candles history Uri.
    /// </summary>
    /// <param name="boardNumber">57</param>
    /// <param name="secId">SecId</param>
    /// <param name="interval">History interval in minutes</param>
    /// <param name="count">Count of intervals. 6 by 10 minutes - is the last hour</param>
    private Uri BuildHistoryUrl(int boardNumber, string secId, int interval, int count = 6)
    {
        var historyPath = $"cs/engines/stock/markets/shares/boardgroups/{boardNumber}/securities/{secId}.hs?s1.type=candles&interval={interval}&candles={count}";
        var uri = new Uri(baseMoexUrl, historyPath, true);
        return uri;
    }

    private Uri BuildCurrentUrl(string secId)
    {
        var currentPath = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{secId}.jsonp?iss.meta=off&lang=ru&iss.only=securities";
        var uri = new Uri(baseMoexUrl, currentPath, true);
        return uri;
    }
}
