using CommunityToolkit.Maui.Views;
using StockWatcher.Models;

namespace StockWatcher.Pages;

public partial class StockSelector : Popup
{
    private List<StockItem> _stockItems;
    private bool _init = true;

    public StockSelector(Size size)
    {
        InitializeComponent();
        _ = FillStockList();
        this.Size = size;
    }

    private async Task FillStockList()
    {
        _stockItems = await ((App)App.Current).Moex.GetStockListAsync();
        ItemsList.ItemsSource = _stockItems;
        _init = false;
    }

    private async void CloseClicked(object sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(null, cts.Token);
    }

    private async void SelectClicked(object sender, EventArgs e)
    {
        if (ItemsList.SelectedItem == null) return;
        var stockItem = ItemsList.SelectedItem as StockItem;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await CloseAsync(stockItem.SecId, cts.Token);
    }

    private void LevelPicker_SelectedIndexChanged(object sender, EventArgs e) => FilterSource();

    private void FilterEntry_TextChanged(object sender, TextChangedEventArgs e) => FilterSource();

    private void FilterSource()
    {
        if (_init) return;

        var items = Enumerable.Empty<StockItem>();
        if (LevelPicker.SelectedIndex == 1)
        {
            items = _stockItems.Where(x => x.Level == 1);
        }
        else if (LevelPicker.SelectedIndex == 2)
        {
            items = _stockItems.Where(x => x.Level == 1 || x.Level == 2);
        }
        else
        {
            items = _stockItems;
        }

        if (!string.IsNullOrEmpty(FilterEntry.Text))
        {
            items = items.Where(x => x.Name.Contains(FilterEntry.Text, StringComparison.InvariantCultureIgnoreCase) ||
                x.SecId.Contains(FilterEntry.Text, StringComparison.InvariantCultureIgnoreCase));
        }

        ItemsList.ItemsSource = items;
    }
}
