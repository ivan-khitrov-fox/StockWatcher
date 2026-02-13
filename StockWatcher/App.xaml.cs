using StockWatcher.Domain;
using StockWatcher.Infrastructure.Api;

namespace StockWatcher;

public partial class App : Application
{
    public MoexClient Moex { get; }
    public MoexMotor Motor { get; }

    public App()
    {
        InitializeComponent();
        Moex = new MoexClient();
        Motor = new MoexMotor(Moex);
        MainPage = new AppShell();
    }
}
