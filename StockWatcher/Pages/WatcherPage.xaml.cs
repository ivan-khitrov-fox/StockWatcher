using CommunityToolkit.Maui.Views;
using StockWatcher.Domain.Watchers;
using StockWatcher.Enums;
using StockWatcher.Infrastructure.Persistence;
using System.Globalization;

namespace StockWatcher.Pages;

[QueryProperty(nameof(WatcherId), "id")]
public partial class WatcherPage : ContentPage
{
    private DataManager _dataManager;
    private string? _watcherId;
    private IWatcher? _loadedWatcher;

    public WatcherPage()
    {
        InitializeComponent();
        _dataManager = ((App)App.Current).DataManager;
        SetupPickers();
    }

    public string? WatcherId
    {
        set
        {
            _watcherId = value;
            LoadWatcher();
        }
    }

    private readonly Dictionary<string, string> _watcherTypes = new Dictionary<string, string>()
    {
        { nameof(LimitWatcher), "Limit watcher" },
        { nameof(DealWatcher), "Deal watcher" }
    };

    private void SetupPickers()
    {
        TypePicker.ItemsSource = _watcherTypes.Values.ToList();
        DirectionPicker.ItemsSource = new string[] { nameof(WatcherType.Up), nameof(WatcherType.Down) };
        // default selection for new watcher
        TypePicker.SelectedIndex = 0;
        DirectionPicker.SelectedIndex = 0;
        UpdateSectionsVisibility();
    }

    private void SaveClicked(object sender, EventArgs e)
    {
        var name = (NameEntry.Text ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            _ = DisplayAlert("Validation", "Name is required.", "OK");
            return;
        }

        var secId = (SecIdButton.Text ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(secId) || secId == "Select Stock Item")
        {
            _ = DisplayAlert("Validation", "Please select SecId.", "OK");
            return;
        }

        var typeLabel = TypePicker.SelectedItem as string;
        var directionLabel = DirectionPicker.SelectedItem as string;
        if (string.IsNullOrWhiteSpace(typeLabel) || string.IsNullOrWhiteSpace(directionLabel))
        {
            _ = DisplayAlert("Validation", "Please select watcher type and direction.", "OK");
            return;
        }

        var direction = (WatcherType)Enum.Parse(typeof(WatcherType), directionLabel);

        if (typeLabel == _watcherTypes[nameof(LimitWatcher)])
        {
            var w = new LimitWatcher
            {
                Id = _loadedWatcher?.Id,
                Name = name,
                SecId = secId,
                Direction = direction,
                DealTime = DateTime.Now
            };

            w.WarningLevel = ParseNullableDouble(WarningLevelEntry.Text);
            w.AlarmLevel = ParseNullableDouble(AlarmLevelEntry.Text);
            w.StartPrice = ParseNullableDouble(StartPriceEntry.Text);
            w.WarningPct = ParseNullableFloat(WarningPctEntry.Text);
            w.AlarmPct = ParseNullableFloat(AlarmPctEntry.Text);

            _dataManager.SaveLimitWatcher(w);
        }
        else
        {
            var price = ParseDouble(DealPriceEntry.Text);
            var count = ParseInt(DealCountEntry.Text);

            if (price <= 0)
            {
                _ = DisplayAlert("Validation", "Deal price must be > 0.", "OK");
                return;
            }
            if (count <= 0)
            {
                _ = DisplayAlert("Validation", "Deal count must be > 0.", "OK");
                return;
            }

            var w = new DealWatcher
            {
                Id = _loadedWatcher?.Id,
                Name = name,
                SecId = secId,
                Direction = direction,
                DealTime = DateTime.Now,
                StockItemPrice = price,
                StockItemCount = count
            };

            _dataManager.SaveDealWatcher(w);
        }

        _ = Shell.Current.GoToAsync("//watchers");
    }

    private void CancelClicked(object sender, EventArgs e)
    {
        _ = Shell.Current.GoToAsync("//watchers");
    }

    private void SecIdButtonClicked(object sender, EventArgs e) => _ = ShowSelector();
    private async Task ShowSelector()
    {
        var size = new Size(300, this.Height * 0.65);
        var stockItemPopup = new StockSelector(size);
        var result = await this.ShowPopupAsync(stockItemPopup);
        if (result != null)
        {
            SecIdButton.Text = (string)result;
        }
    }

    private void TypeChanged(object sender, EventArgs e) => UpdateSectionsVisibility();

    private void UpdateSectionsVisibility()
    {
        var typeLabel = TypePicker.SelectedItem as string;
        var isLimit = typeLabel == _watcherTypes[nameof(LimitWatcher)];
        LimitSection.IsVisible = isLimit;
        DealSection.IsVisible = !isLimit;
    }

    private void LoadWatcher()
    {
        _loadedWatcher = null;

        if (string.IsNullOrWhiteSpace(_watcherId))
        {
            Title = "New watcher";
            NameEntry.Text = string.Empty;
            SecIdButton.Text = "Select Stock Item";
            TypePicker.SelectedIndex = 0;
            DirectionPicker.SelectedIndex = 0;
            ClearTypeSpecificFields();
            UpdateSectionsVisibility();
            return;
        }

        Title = "Edit watcher";

        // Expect ids like "L12" or "D3"
        var idStr = _watcherId.Trim();
        var prefix = idStr[0];
        _ = int.TryParse(idStr.Substring(1), out var numericId);

        if (numericId <= 0)
            return;

        if (prefix == 'L')
            _loadedWatcher = _dataManager.GetLimitWatcher(numericId);
        else if (prefix == 'D')
            _loadedWatcher = _dataManager.GetDealWatcher(numericId);

        if (_loadedWatcher == null)
            return;

        NameEntry.Text = _loadedWatcher.Name;
        SecIdButton.Text = _loadedWatcher.SecId;
        DirectionPicker.SelectedItem = _loadedWatcher.Direction.ToString();

        if (_loadedWatcher is LimitWatcher lw)
        {
            TypePicker.SelectedItem = _watcherTypes[nameof(LimitWatcher)];
            WarningLevelEntry.Text = lw.WarningLevel?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            AlarmLevelEntry.Text = lw.AlarmLevel?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            StartPriceEntry.Text = lw.StartPrice?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            WarningPctEntry.Text = lw.WarningPct?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            AlarmPctEntry.Text = lw.AlarmPct?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        }
        else if (_loadedWatcher is DealWatcher dw)
        {
            TypePicker.SelectedItem = _watcherTypes[nameof(DealWatcher)];
            DealPriceEntry.Text = dw.StockItemPrice.ToString(CultureInfo.InvariantCulture);
            DealCountEntry.Text = dw.StockItemCount.ToString(CultureInfo.InvariantCulture);
        }

        UpdateSectionsVisibility();
    }

    private void ClearTypeSpecificFields()
    {
        DealPriceEntry.Text = string.Empty;
        DealCountEntry.Text = string.Empty;
        WarningLevelEntry.Text = string.Empty;
        AlarmLevelEntry.Text = string.Empty;
        StartPriceEntry.Text = string.Empty;
        WarningPctEntry.Text = string.Empty;
        AlarmPctEntry.Text = string.Empty;
    }

    private static double ParseDouble(string? s)
    {
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
            return v;
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
            return v;
        return 0;
    }

    private static int ParseInt(string? s)
    {
        if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
            return v;
        if (int.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
            return v;
        return 0;
    }

    private static double? ParseNullableDouble(string? s)
    {
        var v = ParseDouble(s);
        return v == 0 ? null : v;
    }

    private static float? ParseNullableFloat(string? s)
    {
        if (float.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
            return v == 0 ? null : v;
        if (float.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
            return v == 0 ? null : v;
        return null;
    }
}
