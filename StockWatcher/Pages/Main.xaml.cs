using StockWatcher.Core;

namespace StockWatcher.Pages;

public partial class Main : ContentPage
{
    private MoexRequests _moexRequests;

    public Main()
	{
		InitializeComponent();
        _moexRequests = ((App)App.Current).MoexRequests;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = OnAppearingAsync();
    }

    private async Task OnAppearingAsync()
	{
        var secId = "SBER";
        var hourHistory = await _moexRequests.GetLastHourHistory(secId);

        foreach (var x in hourHistory)
        {
            var s = x.SecId + ": " + x.StartTime.ToString() + ": " + x.StartPrice.ToString() + " - " + x.EndPrice.ToString();
            var lab = new Label { Text = s };
            V.Add(lab);
        }
    }
}
