using StockWatcher.Core;

namespace StockWatcher;

public partial class App : Application
{
    public MoexRequests MoexRequests { get; }
    public MoexMotor Motor { get; }

    public App()
    {
        InitializeComponent();
        MoexRequests = new MoexRequests();
        Motor = new MoexMotor(MoexRequests);
        MainPage = new AppShell();
    }
}
