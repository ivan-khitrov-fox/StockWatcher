using WData = Windows.Data;
using Windows.UI.Notifications;
using StockWatcher.Services;

namespace StockWatcher.Platforms.Windows.Services
{
    public class WindowsNotificationService : INotificationService
    {
        public Task NotifyAsync(string message)
        {
            ShowToast("StockWatcher", message);
            return Task.CompletedTask;
        }

        public Task NotifyLimitHitAsync(string secId, string message)
        {
            ShowToast($"Limit Hit: {secId}", message);
            return Task.CompletedTask;
        }

        private void ShowToast(string title, string message)
        {
            var xml = $@"
            <toast>
                <visual>
                    <binding template='ToastGeneric'>
                        <text>{title}</text>
                        <text>{message}</text>
                    </binding>
                </visual>
            </toast>";

            var doc = new WData.Xml.Dom.XmlDocument();
            doc.LoadXml(xml);

            var toast = new ToastNotification(doc);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
