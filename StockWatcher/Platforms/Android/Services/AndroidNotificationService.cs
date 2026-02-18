using Android.App;
using Android.Content;
using AOS = Android.OS;
using AndroidX.Core.App;
using StockWatcher.Services;

namespace StockWatcher.Platforms.Android.Services
{
    public class AndroidNotificationService : INotificationService
    {
        const string ChannelId = "stockwatcher_notifications";
        const string ChannelName = "StockWatcher Notifications";
        const string ChannelDescription = "Notifications for StockWatcher app";

        private readonly Context _context;

        public AndroidNotificationService(Context context)
        {
            _context = context;
            CreateNotificationChannel();
        }

        public Task NotifyAsync(string message)
        {
            ShowNotification("StockWatcher", message);
            return Task.CompletedTask;
        }

        public Task NotifyLimitHitAsync(string secId, string message)
        {
            ShowNotification($"Limit Hit: {secId}", message);
            return Task.CompletedTask;
        }

        private void ShowNotification(string title, string message)
        {
            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
               //////////////// .SetSmallIcon(Resource.Drawable.ic_stat_name) // replace with your icon
                .SetAutoCancel(true);

            var notificationManager = NotificationManagerCompat.From(_context);
            notificationManager.Notify(new Random().Next(), builder.Build());
        }

        private void CreateNotificationChannel()
        {
            if (AOS.Build.VERSION.SdkInt >= AOS.BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, ChannelName, NotificationImportance.Default)
                {
                    Description = ChannelDescription
                };

                var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}
