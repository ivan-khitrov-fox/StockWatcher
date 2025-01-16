using StockWatcher.Core.Data;
using StockWatcher.Core.Watchers;

namespace StockWatcher.Core;

public class MoexMotor
{
    private List<IWatcher> _watchers;
    private List<IAdvisor> _advisors;
    private MoexRequests _requests;

    public bool Disabled { get; set; }
    public bool DisabledByTime => CheckDisabledTime();

    public TimeSpan? WorkTimeStart { get; private set; }
    public TimeSpan? WorkTimeEnd { get; private set; }

    public DataManager DataManager { get; private set; }

    public MoexMotor(MoexRequests moexRequests)
    {
        _requests = moexRequests;
        DataManager = new DataManager();
    }

    public void Init()
    {

    }

    public async Task Run()
    {
        if (Disabled) return;
        if (DisabledByTime) return;

        var activeWatchers = _watchers.Where(x => !x.Disabled);
        var reactions = new List<WatcherReaction>();

        foreach (var watcher in activeWatchers)
        {
            var data = await _requests.GetLastHourHistory(watcher.SecId);
            var reaction = await watcher.Analyze(data);
            reactions.Add(reaction);
        }

        var activeAdvisors = _advisors.Where(x => !x.Disabled && !activeWatchers.Any(w => w.SecId == x.SecId));
        foreach (var advisor in activeAdvisors)
        {
            var reaction = await advisor.Analyze();
            reactions.Add(reaction);
        }

        HandleReactions(reactions);
    }

    private bool CheckDisabledTime()
    {
        var time = DateTime.Now.TimeOfDay;
        if (WorkTimeStart.HasValue && time < WorkTimeStart.Value) return true;
        if (WorkTimeEnd.HasValue && time > WorkTimeEnd.Value) return true;
        return false;
    }

    private void HandleReactions(List<WatcherReaction> reactions)
    {

    }
}
