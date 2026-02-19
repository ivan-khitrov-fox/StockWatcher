using StockWatcher.Domain.Watchers;
using StockWatcher.Enums;
using StockWatcher.Infrastructure.Api;
using StockWatcher.Infrastructure.Persistence;

namespace StockWatcher.Services;

public class StockPollingWorker
{
    private readonly MoexClient _moexClient;
    private readonly DataManager _dataManager;
    private readonly INotificationService _notificationService;

    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    public bool Disabled { get; set; }
    public bool DisabledByTime => CheckDisabledTime();
    public TimeSpan? WorkTimeStart { get; private set; }
    public TimeSpan? WorkTimeEnd { get; private set; }

    public StockPollingWorker(
        MoexClient moexClient,
        DataManager dataManager,
        INotificationService notificationService)
    {
        _moexClient = moexClient;
        _dataManager = dataManager;
        _notificationService = notificationService;
    }

    public void Start()
    {
        if (_cts != null)
            return;

        _cts = new CancellationTokenSource();
        _loopTask = Task.Run(() => RunLoop(_cts.Token));
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts = null;
    }

    // =========================
    // 🔁 MAIN POLLING LOOP
    // =========================
    private async Task RunLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (Disabled || DisabledByTime)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), token);
                    continue;
                }

                var limitWatchers = _dataManager.GetLimitWatchers();
                var dealWatchers = _dataManager.GetDealWatchers();
                var watchers = limitWatchers.Cast<IWatcher>().Union(dealWatchers).ToList();

                foreach (var watcher in watchers)
                {
                    try
                    {
                        var history = await _moexClient.GetLastHourHistoryAsync(watcher.SecId, token);

                        if (history == null || history.Count == 0)
                            continue;

                        var reaction = watcher.Analyze(history.Last());

                        if (reaction.Type != ReactionType.Keep)
                        {
                            try
                            {
                                await _notificationService.NotifyLimitHitAsync(
                                    reaction.SecId,
                                    reaction.Message);
                            }
                            catch (Exception notifyEx)
                            {
                                System.Diagnostics.Debug.WriteLine(notifyEx);
                            }
                        }
                    }
                    catch (Exception watcherEx)
                    {
                        // Isolate per-security failures: continue polling the rest.
                        System.Diagnostics.Debug.WriteLine(watcherEx);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), token);
        }
    }

    private bool CheckDisabledTime()
    {
        var time = DateTime.Now.TimeOfDay;
        if (WorkTimeStart.HasValue && time < WorkTimeStart.Value) return true;
        if (WorkTimeEnd.HasValue && time > WorkTimeEnd.Value) return true;
        return false;
    }
}
