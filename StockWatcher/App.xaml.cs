using StockWatcher.Infrastructure.Api;
using StockWatcher.Infrastructure.Persistence;
using StockWatcher.Services;

namespace StockWatcher;

public partial class App : Application
{
    public MoexClient Moex { get; }

    public DataManager DataManager { get; }

    public INotificationService NotificationService { get; }

    public StockPollingWorker PollingWorker { get; }

    public App()
    {
        InitializeComponent();
        Moex = new MoexClient();
        DataManager = new DataManager();
        NotificationService = null;
        PollingWorker = new StockPollingWorker(Moex, DataManager, NotificationService);
        MainPage = new AppShell();
    }
}
