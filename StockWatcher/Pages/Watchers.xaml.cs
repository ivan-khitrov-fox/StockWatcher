using StockWatcher.Domain.Watchers;
using StockWatcher.CustomViews;
using StockWatcher.Infrastructure.Persistence;
using System.Collections.ObjectModel;

namespace StockWatcher.Pages;

public partial class Watchers : ContentPage
{
    private DataManager _dataManager;
    public ObservableCollection<IWatcher> WatchersList { get; set; }

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
        _dataManager = ((App)App.Current).DataManager;
        WatchersList = new ObservableCollection<IWatcher>();
        WatchersCollectionView.ItemTemplate = new DataTemplate(() => new WatcherCell());
    }

    private void LoadWatchers()
    {
        WatchersList.Clear();
        var dealWatchers = _dataManager.GetDealWatchers();
        var limitWatchers = _dataManager.GetLimitWatchers();
        var watchers = dealWatchers.Cast<IWatcher>().Union(limitWatchers).ToList();
        foreach (var w in watchers)
            WatchersList.Add(w);
    }

    private void AddWatcherClicked(object sender, EventArgs e)
    {
        _ = Shell.Current.GoToAsync("//watcher");
    }

    private void WatcherSelected(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            var watcher = e.CurrentSelection?.FirstOrDefault() as IWatcher;
            if (watcher == null)
                return;

            _ = Shell.Current.GoToAsync("//watcher", new ShellNavigationQueryParameters { { "id", watcher.Id } });
        }
        finally
        {
            // allow re-selecting same item later
            WatchersCollectionView.SelectedItem = null;
        }
    }
}
