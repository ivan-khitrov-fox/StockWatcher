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

    public App(
        MoexClient moex,
        DataManager dataManager,
        INotificationService notificationService,
        StockPollingWorker pollingWorker)
    {
        InitializeComponent();
        Moex = moex;
        DataManager = dataManager;
        NotificationService = notificationService;
        PollingWorker = pollingWorker;
        MainPage = new AppShell();
    }
}
