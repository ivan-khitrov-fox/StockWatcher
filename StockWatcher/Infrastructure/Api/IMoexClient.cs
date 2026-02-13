using StockWatcher.Models;

namespace StockWatcher.Infrastructure.Api;

public interface IMoexClient
{
    /// <summary>
    /// Retrieves the list of stocks from MOEX.
    /// </summary>
    Task<List<StockItem>> GetStockListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the last hour of historical candle data for the given security.
    /// </summary>
    /// <param name="secId">Security identifier</param>
    Task<List<HourHistory>> GetLastHourHistoryAsync(string secId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current price for the given security.
    /// </summary>
    /// <param name="secId">Security identifier</param>
    Task<CurrentPrice> GetCurrentAsync(string secId, CancellationToken cancellationToken = default);
}
