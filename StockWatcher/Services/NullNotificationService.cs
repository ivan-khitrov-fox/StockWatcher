namespace StockWatcher.Services;

/// <summary>
/// No-op implementation for platforms without native notification support (e.g. iOS).
/// </summary>
public class NullNotificationService : INotificationService
{
    public Task NotifyAsync(string message) => Task.CompletedTask;

    public Task NotifyLimitHitAsync(string secId, string message) => Task.CompletedTask;
}
