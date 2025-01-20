using StockWatcher.Core.Data;
using StockWatcher.Core.Watchers;
using StockWatcher.CustomViews;

namespace StockWatcher.Pages;


public partial class Watchers : ContentPage
{
    private DataManager _dataManager;
    public List<IWatcher> WatchersList { get; set; }

    public Watchers()
    {
        InitializeComponent();
        this.BindingContext = this;
        Init();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadWatchers();
    }

    private void Init()
    {
        _dataManager = ((App)App.Current).Motor.DataManager;
        WatchersList = new List<IWatcher>();
        WatchersCollectionView.ItemTemplate = new DataTemplate(() => new WatcherCell());
    }

    private void LoadWatchers()
    {
        var dealWatchers = _dataManager.GetDealWatchers();
        var limitWatchers = _dataManager.GetLimitWatchers();
        var watchers = dealWatchers.Cast<IWatcher>().Union(limitWatchers).ToList();
        watchers.ForEach(WatchersList.Add);
    }

    private void AddWatcherClicked(object sender, EventArgs e)
    {
        _ = Shell.Current.GoToAsync("//watcher",  new ShellNavigationQueryParameters { { "id", 1 } });
    }
}
