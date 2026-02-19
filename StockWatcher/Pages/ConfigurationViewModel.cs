using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using StockWatcher.Utilities;

namespace StockWatcher.Pages;

public class ConfigurationViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private int _moexBoardNumber;
    private int _moexHistoryIntervalMinutes;
    private string _defaultDemoSecId = string.Empty;
    private int _pollingIntervalMinutes;
    private int _pollingIdleIntervalMinutes;
    private float _dealWatcherWarningPctDefault;
    private float _dealWatcherAlarmPctDefault;
    private string _errorText = string.Empty;
    private bool _isBusy;

    private static readonly Regex SecIdRegex = new Regex("^[A-Z0-9._-]{1,20}$", RegexOptions.Compiled);

    public ConfigurationViewModel()
    {
        LoadFromSettings();
        Validate();
    }

    public int MoexBoardNumber
    {
        get => _moexBoardNumber;
        set => Set(ref _moexBoardNumber, value);
    }

    public int MoexHistoryIntervalMinutes
    {
        get => _moexHistoryIntervalMinutes;
        set => Set(ref _moexHistoryIntervalMinutes, value);
    }

    public string DefaultDemoSecId
    {
        get => _defaultDemoSecId;
        set => Set(ref _defaultDemoSecId, string.IsNullOrWhiteSpace(value) ? AppConfig.DefaultDemoSecId : value.Trim().ToUpperInvariant());
    }

    public int PollingIntervalMinutes
    {
        get => _pollingIntervalMinutes;
        set => Set(ref _pollingIntervalMinutes, value);
    }

    public int PollingIdleIntervalMinutes
    {
        get => _pollingIdleIntervalMinutes;
        set => Set(ref _pollingIdleIntervalMinutes, value);
    }

    public float DealWatcherWarningPctDefault
    {
        get => _dealWatcherWarningPctDefault;
        set => Set(ref _dealWatcherWarningPctDefault, value);
    }

    public float DealWatcherAlarmPctDefault
    {
        get => _dealWatcherAlarmPctDefault;
        set => Set(ref _dealWatcherAlarmPctDefault, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanSave));
            OnPropertyChanged(nameof(CanTest));
            OnPropertyChanged(nameof(CanReset));
        }
    }

    public string ErrorText
    {
        get => _errorText;
        private set
        {
            if (_errorText == value) return;
            _errorText = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(CanSave));
            OnPropertyChanged(nameof(CanTest));
        }
    }

    public bool HasErrors => !string.IsNullOrWhiteSpace(ErrorText);
    public bool CanSave => !IsBusy && !HasErrors;
    public bool CanTest => !IsBusy && !HasErrors;
    public bool CanReset => !IsBusy;

    public bool Validate()
    {
        var errors = new List<string>();

        if (MoexBoardNumber < 1 || MoexBoardNumber > 999)
            errors.Add("MOEX board number must be between 1 and 999.");

        if (MoexHistoryIntervalMinutes < 1 || MoexHistoryIntervalMinutes > 60)
            errors.Add("History interval must be between 1 and 60 minutes.");

        if (PollingIntervalMinutes < 1 || PollingIntervalMinutes > 1440)
            errors.Add("Polling interval must be between 1 and 1440 minutes.");

        if (PollingIdleIntervalMinutes < 1 || PollingIdleIntervalMinutes > 1440)
            errors.Add("Idle interval must be between 1 and 1440 minutes.");

        if (PollingIdleIntervalMinutes > PollingIntervalMinutes)
            errors.Add("Idle interval must be <= polling interval.");

        if (DealWatcherWarningPctDefault < 0 || DealWatcherWarningPctDefault > 1000)
            errors.Add("DealWatcher warning % must be between 0 and 1000.");

        if (DealWatcherAlarmPctDefault < 0 || DealWatcherAlarmPctDefault > 1000)
            errors.Add("DealWatcher alarm % must be between 0 and 1000.");

        if (DealWatcherAlarmPctDefault < DealWatcherWarningPctDefault)
            errors.Add("DealWatcher alarm % must be >= warning %.");

        if (string.IsNullOrWhiteSpace(DefaultDemoSecId) || !SecIdRegex.IsMatch(DefaultDemoSecId))
            errors.Add("Demo SecId must match: A-Z, 0-9, dot, underscore, dash (1-20 chars).");

        ErrorText = string.Join(Environment.NewLine, errors);
        return errors.Count == 0;
    }

    public void Save()
    {
        if (!Validate())
            return;

        AppSettings.MoexBoardNumber = MoexBoardNumber;
        AppSettings.MoexHistoryIntervalMinutes = MoexHistoryIntervalMinutes;
        AppSettings.DefaultDemoSecId = DefaultDemoSecId;
        AppSettings.PollingInterval = TimeSpan.FromMinutes(PollingIntervalMinutes);
        AppSettings.PollingIdleInterval = TimeSpan.FromMinutes(PollingIdleIntervalMinutes);
        AppSettings.DealWatcherWarningPctDefault = DealWatcherWarningPctDefault;
        AppSettings.DealWatcherAlarmPctDefault = DealWatcherAlarmPctDefault;
    }

    public void Reset()
    {
        AppSettings.ResetToDefaults();
        LoadFromSettings();
        Validate();
    }

    private void LoadFromSettings()
    {
        _moexBoardNumber = AppSettings.MoexBoardNumber;
        _moexHistoryIntervalMinutes = AppSettings.MoexHistoryIntervalMinutes;
        _defaultDemoSecId = AppSettings.DefaultDemoSecId;
        _pollingIntervalMinutes = (int)AppSettings.PollingInterval.TotalMinutes;
        _pollingIdleIntervalMinutes = (int)AppSettings.PollingIdleInterval.TotalMinutes;
        _dealWatcherWarningPctDefault = AppSettings.DealWatcherWarningPctDefault;
        _dealWatcherAlarmPctDefault = AppSettings.DealWatcherAlarmPctDefault;

        OnPropertyChanged(string.Empty);
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return;
        field = value;
        OnPropertyChanged(propertyName);
        Validate();
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

