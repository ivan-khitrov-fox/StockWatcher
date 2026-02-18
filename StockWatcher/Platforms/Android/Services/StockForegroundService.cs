using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using StockWatcher.Services;

namespace StockWatcher.Platforms.Android.Services;

[Service(Exported = false)]
public class StockForegroundService : Service
{
    public const string ChannelId = "stock_watcher_foreground";

    public override void OnCreate()
    {
        base.OnCreate();
        CreateNotificationChannel();
    }

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        StartForeground(1001, BuildNotification());

        // ⭐ start shared worker
        var worker = IPlatformApplication.Current!.Services.GetService<StockPollingWorker>();
        worker?.Start();

        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        var worker = IPlatformApplication.Current!.Services.GetService<StockPollingWorker>();
        worker?.Stop();

        base.OnDestroy();
    }

    public override IBinder? OnBind(Intent? intent) => null;

    // =========================
    // 🔔 Notification
    // =========================
    private Notification BuildNotification()
    {
        return new NotificationCompat.Builder(this, ChannelId)
            .SetContentTitle("StockWatcher running")
            .SetContentText("Monitoring stock limits")
           // .SetSmallIcon(Android.Resource.Drawable.IcDialogInfo)
            .SetOngoing(true)
            .Build();
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var channel = new NotificationChannel(
            ChannelId,
            "Stock Watcher Background",
            NotificationImportance.Low);

        var manager = (NotificationManager)GetSystemService(NotificationService)!;
        manager.CreateNotificationChannel(channel);
    }
}
