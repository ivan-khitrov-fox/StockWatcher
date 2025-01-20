using StockWatcher.Core.Data;
using StockWatcher.Core.Watchers;
using StockWatcher.Enums;
using StockWatcher.Models;

namespace StockWatcher.Pages;

[QueryProperty(nameof(WatcherId), "id")]
public partial class WatcherPage : ContentPage
{
    private DataManager _dataManager;
    private int? _watcherId;

    public WatcherPage()
    {
        InitializeComponent();
        this.BindingContext = this;
    }

    public object WatcherId
    {
        set 
        { 
            _watcherId = (int?)value;
            SetupWatcher();
        } 
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = FillStockList();
    }

    private async Task FillStockList()
    {
        var stockItems = await ((App)App.Current).MoexRequests.GetStoclList();
        SecIdPicker.ItemsSource = stockItems;
    }

    private readonly Dictionary<string, string> _watcherTypes = new Dictionary<string, string>()
    {
        { nameof(LimitWatcher), "Limit watcher" },
        { nameof(DealWatcher), "Deal watcher" }
    };

    private void SetupWatcher()
    {
        TypePicker.ItemsSource = _watcherTypes.Values.ToList();
        DirectionPicker.ItemsSource = new string[] { nameof(WatcherType.Up), nameof(WatcherType.Down) };
        
    }

    private void SaveClicked(object sender, EventArgs e)
    {
        _ = Shell.Current.GoToAsync("//watchers");
    }

    private void CancelClicked(object sender, EventArgs e)
    {
        _ = Shell.Current.GoToAsync("//watchers");
    }
}
