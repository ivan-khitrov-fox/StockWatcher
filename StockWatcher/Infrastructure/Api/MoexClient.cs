using System.Net.Http.Headers;
using System.Net;
using StockWatcher.Converters;
using StockWatcher.Models;

namespace StockWatcher.Infrastructure.Api;

public class MoexClient
{
    private readonly HttpClient _httpClient;
    private readonly Uri _baseMoexUrl = new Uri("https://iss.moex.com");
    private readonly int _boardNumber = 57;
    private readonly int _historyIntervalInMinutes = 10;
    private const int DefaultMaxRetries = 3;

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
        var responseString = await GetStringWithRetryAsync(uri, cancellationToken);
        return RawStockListConverter.ToStockList(responseString);
    }

    public async Task<List<HourHistory>> GetLastHourHistoryAsync(string secId, CancellationToken cancellationToken = default)
    {
        var uri = BuildHistoryUrl(_boardNumber, secId, _historyIntervalInMinutes);
        var responseString = await GetStringWithRetryAsync(uri, cancellationToken);
        return RawCandleDataConverter.ToCandleData(secId, _historyIntervalInMinutes, responseString);
    }

    public async Task<CurrentPrice> GetCurrentAsync(string secId, CancellationToken cancellationToken = default)
    {
        var uri = BuildCurrentUrl(secId);
        var responseString = await GetStringWithRetryAsync(uri, cancellationToken);
        return RawCurrentPriceDataConverter.ToCurrentPriceData(responseString);
    }

    private async Task<string> GetStringWithRetryAsync(Uri uri, CancellationToken cancellationToken, int maxRetries = DefaultMaxRetries)
    {
        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync(cancellationToken);

                if (!IsRetryable(response.StatusCode) || attempt == maxRetries)
                {
                    var body = await SafeReadBodyAsync(response, cancellationToken);
                    throw new HttpRequestException(
                        $"MOEX request failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}",
                        inner: null,
                        statusCode: response.StatusCode);
                }
            }
            catch (Exception ex) when (IsTransientException(ex) && attempt < maxRetries)
            {
                // fall through to delay & retry
            }

            await Task.Delay(GetBackoffDelay(attempt), cancellationToken);
        }

        // Should never reach here
        throw new HttpRequestException("MOEX request failed after retries.");
    }

    private static bool IsRetryable(HttpStatusCode statusCode) =>
        statusCode == (HttpStatusCode)429 ||
        (int)statusCode >= 500;

    private static bool IsTransientException(Exception ex)
    {
        // Timeout: TaskCanceledException is also used for user cancellations; we rely on the caller token check above.
        if (ex is HttpRequestException) return true;
        if (ex is IOException) return true;
        if (ex is TaskCanceledException) return true;
        return false;
    }

    private static TimeSpan GetBackoffDelay(int attempt)
    {
        // Exponential backoff with small jitter: 250ms, 500ms, 1000ms, ...
        var baseMs = 250 * Math.Pow(2, attempt);
        var jitterMs = Random.Shared.Next(0, 150);
        var delayMs = Math.Min(4000, (int)baseMs + jitterMs);
        return TimeSpan.FromMilliseconds(delayMs);
    }

    private static async Task<string> SafeReadBodyAsync(HttpResponseMessage response, CancellationToken token)
    {
        try
        {
            return await response.Content.ReadAsStringAsync(token);
        }
        catch
        {
            return "<unavailable>";
        }
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
