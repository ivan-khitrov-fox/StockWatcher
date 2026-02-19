namespace StockWatcher.Utilities;

/// <summary>
/// Central place for application-wide configuration and defaults.
/// For now values заданы в коде, но при желании можно вынести в хранимые настройки/файл.
/// </summary>
public static class AppConfig
{
    // MOEX-related settings
    public const int MoexBoardNumber = 57;
    public const int MoexHistoryIntervalMinutes = 10;

    // UI / demo defaults
    public const string DefaultDemoSecId = "SBER";

    // Background polling
    public static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan PollingIdleInterval = TimeSpan.FromMinutes(1);

    // Deal watcher defaults
    public const float DealWatcherWarningPctDefault = 1f;
    public const float DealWatcherAlarmPctDefault = 3f;
}

