namespace StockWatcher.Services;

public interface INotificationService
{
    Task NotifyAsync(string message);
    Task NotifyLimitHitAsync(string secId, string message);
}
