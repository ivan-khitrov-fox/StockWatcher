namespace StockWatcher.Pages;

public partial class Configuration : ContentPage
{
    private readonly ConfigurationViewModel _vm;

	public Configuration()
	{
		InitializeComponent();
        _vm = new ConfigurationViewModel();
        BindingContext = _vm;
	}

    private async void SaveClicked(object sender, EventArgs e)
    {
        _vm.Save();
        await DisplayAlert("Saved", "Settings have been saved.", "OK");
    }

    private async void ResetClicked(object sender, EventArgs e)
    {
        _vm.Reset();
        await DisplayAlert("Reset", "Settings restored to defaults.", "OK");
    }

    private async void TestClicked(object sender, EventArgs e)
    {
        if (!_vm.Validate())
        {
            await DisplayAlert("Invalid settings", "Please fix the validation errors and try again.", "OK");
            return;
        }

        try
        {
            _vm.IsBusy = true;

            var moex = ((App)App.Current).Moex;
            var history = await moex.GetLastHourHistoryAsync(
                _vm.DefaultDemoSecId,
                _vm.MoexBoardNumber,
                _vm.MoexHistoryIntervalMinutes,
                CancellationToken.None);

            if (history == null || history.Count == 0)
            {
                await DisplayAlert("MOEX test", "Request succeeded, but history is empty.", "OK");
                return;
            }

            var last = history.Last();
            await DisplayAlert(
                "MOEX test OK",
                $"{last.SecId}: {last.EndTime:g}\n{last.StartPrice} → {last.EndPrice} ({history.Count} candles)",
                "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("MOEX test failed", ex.Message, "OK");
        }
        finally
        {
            _vm.IsBusy = false;
        }
    }
}