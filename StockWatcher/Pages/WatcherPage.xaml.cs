using CommunityToolkit.Maui.Views;
using StockWatcher.Domain.Watchers;
using StockWatcher.Enums;
using StockWatcher.Infrastructure.Persistence;

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
}
