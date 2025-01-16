using StockWatcher.Core.Data;
using StockWatcher.Core.Watchers;
using StockWatcher.CustomViews;

namespace StockWatcher.Pages;

public partial class Watchers : ContentPage
{
    private DataManager _dataManager;
    public List<DealWatcher> WatchersList { get; set; }

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
        WatchersList = new List<DealWatcher>();
        WatchersCollectionView.ItemTemplate = new DataTemplate(() => new WatcherCell());
    }

    private void LoadWatchers()
    {
        var watchers = _dataManager.GetDealWatchers();
        watchers.ForEach(WatchersList.Add);
    }
}
