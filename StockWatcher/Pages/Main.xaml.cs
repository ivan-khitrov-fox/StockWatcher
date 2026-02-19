using StockWatcher.Infrastructure.Api;
using StockWatcher.Utilities;

namespace StockWatcher.Pages;

public partial class Main : ContentPage
{
    private MoexClient _moexRequests;

    public Main()
	{
		InitializeComponent();
        _moexRequests = ((App)App.Current).Moex;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = OnAppearingAsync();
    }

    private async Task OnAppearingAsync()
	{
        var secId = AppSettings.DefaultDemoSecId;
        var hourHistory = await _moexRequests.GetLastHourHistoryAsync(secId);

        foreach (var x in hourHistory)
        {
            var s = x.SecId + ": " + x.StartTime.ToString() + ": " + x.StartPrice.ToString() + " - " + x.EndPrice.ToString();
            var lab = new Label { Text = s };
            V.Add(lab);
        }
    }
}
