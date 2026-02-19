using Microsoft.Maui.Storage;

namespace StockWatcher.Utilities;

/// <summary>
/// Persistent user-editable settings stored in MAUI Preferences.
/// Defaults are defined in <see cref="AppConfig"/>.
/// </summary>
public static class AppSettings
{
    private static class Keys
    {
        public const string MoexBoardNumber = "settings.moex.boardNumber";
        public const string MoexHistoryIntervalMinutes = "settings.moex.historyIntervalMinutes";
        public const string DefaultDemoSecId = "settings.ui.defaultDemoSecId";

        public const string PollingIntervalMinutes = "settings.polling.intervalMinutes";
        public const string PollingIdleIntervalMinutes = "settings.polling.idleIntervalMinutes";

        public const string DealWatcherWarningPctDefault = "settings.dealWatcher.warningPctDefault";
        public const string DealWatcherAlarmPctDefault = "settings.dealWatcher.alarmPctDefault";
    }

    public static int MoexBoardNumber
    {
        get => Preferences.Get(Keys.MoexBoardNumber, AppConfig.MoexBoardNumber);
        set => Preferences.Set(Keys.MoexBoardNumber, value);
    }

    public static int MoexHistoryIntervalMinutes
    {
        get => Preferences.Get(Keys.MoexHistoryIntervalMinutes, AppConfig.MoexHistoryIntervalMinutes);
        set => Preferences.Set(Keys.MoexHistoryIntervalMinutes, value);
    }

    public static string DefaultDemoSecId
    {
        get => Preferences.Get(Keys.DefaultDemoSecId, AppConfig.DefaultDemoSecId);
        set => Preferences.Set(Keys.DefaultDemoSecId, value ?? AppConfig.DefaultDemoSecId);
    }

    public static TimeSpan PollingInterval
    {
        get => TimeSpan.FromMinutes(Preferences.Get(Keys.PollingIntervalMinutes, (int)AppConfig.PollingInterval.TotalMinutes));
        set => Preferences.Set(Keys.PollingIntervalMinutes, (int)Math.Max(1, value.TotalMinutes));
    }

    public static TimeSpan PollingIdleInterval
    {
        get => TimeSpan.FromMinutes(Preferences.Get(Keys.PollingIdleIntervalMinutes, (int)AppConfig.PollingIdleInterval.TotalMinutes));
        set => Preferences.Set(Keys.PollingIdleIntervalMinutes, (int)Math.Max(1, value.TotalMinutes));
    }

    public static float DealWatcherWarningPctDefault
    {
        get => Preferences.Get(Keys.DealWatcherWarningPctDefault, AppConfig.DealWatcherWarningPctDefault);
        set => Preferences.Set(Keys.DealWatcherWarningPctDefault, value);
    }

    public static float DealWatcherAlarmPctDefault
    {
        get => Preferences.Get(Keys.DealWatcherAlarmPctDefault, AppConfig.DealWatcherAlarmPctDefault);
        set => Preferences.Set(Keys.DealWatcherAlarmPctDefault, value);
    }

    public static void ResetToDefaults()
    {
        Preferences.Remove(Keys.MoexBoardNumber);
        Preferences.Remove(Keys.MoexHistoryIntervalMinutes);
        Preferences.Remove(Keys.DefaultDemoSecId);
        Preferences.Remove(Keys.PollingIntervalMinutes);
        Preferences.Remove(Keys.PollingIdleIntervalMinutes);
        Preferences.Remove(Keys.DealWatcherWarningPctDefault);
        Preferences.Remove(Keys.DealWatcherAlarmPctDefault);
    }
}

