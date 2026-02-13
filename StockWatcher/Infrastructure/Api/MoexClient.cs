using System.Net.Http.Headers;
using StockWatcher.Converters;
using StockWatcher.Models;

namespace StockWatcher.Infrastructure.Api;

public class MoexClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseMoexUrl = new Uri("https://iss.moex.com");
    private readonly int _boardNumber = 57;
    private readonly int _historyIntervalInMinutes = 10;

    public MoexClient()
    {
        _httpClient = new HttpClient();

        // Ensure JSON and plain text are accepted
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
        _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<List<StockItem>> GetStockListAsync(CancellationToken cancellationToken = default)
    {
        var uri = BuildListUrl(_boardNumber);
        using var responseStream = await GetStreamAsync(uri, cancellationToken);
        var responseString = await new StreamReader(responseStream).ReadToEndAsync();
        return RawStockListConverter.ToStockList(responseString);
    }

    public async Task<List<HourHistory>> GetLastHourHistoryAsync(string secId, CancellationToken cancellationToken = default)
    {
        var uri = BuildHistoryUrl(_boardNumber, secId, _historyIntervalInMinutes);
        using var responseStream = await GetStreamAsync(uri, cancellationToken);
        var responseString = await new StreamReader(responseStream).ReadToEndAsync();
        return RawCandleDataConverter.ToCandleData(secId, _historyIntervalInMinutes, responseString);
    }

    public async Task<CurrentPrice> GetCurrentAsync(string secId, CancellationToken cancellationToken = default)
    {
        var uri = BuildCurrentUrl(secId);
        using var responseStream = await GetStreamAsync(uri, cancellationToken);
        var responseString = await new StreamReader(responseStream).ReadToEndAsync();
        return RawCurrentPriceDataConverter.ToCurrentPriceData(responseString);
    }

    /// <summary>
    /// Centralized HTTP GET with streaming and status check
    /// </summary>
    private async Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    private Uri BuildListUrl(int boardNumber)
    {
        var path = $"iss/engines/stock/markets/shares/boardgroups/{boardNumber}/securities.json?iss.meta=off&lang=ru&security_collection=3&sort_column=SHORTNAME&sort_order=asc";
        return new Uri(_baseMoexUrl, path, true);
    }

    private Uri BuildHistoryUrl(int boardNumber, string secId, int interval, int count = 6)
    {
        var path = $"cs/engines/stock/markets/shares/boardgroups/{boardNumber}/securities/{secId}.hs?s1.type=candles&interval={interval}&candles={count}";
        return new Uri(_baseMoexUrl, path, true);
    }

    private Uri BuildCurrentUrl(string secId)
    {
        var path = $"iss/engines/stock/markets/shares/boards/TQBR/securities/{secId}.jsonp?iss.meta=off&lang=ru&iss.only=securities";
        return new Uri(_baseMoexUrl, path, true);
    }
}
