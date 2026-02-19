using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using StockWatcher.Infrastructure.Api;
using StockWatcher.Infrastructure.Persistence;
using StockWatcher.Services;

namespace StockWatcher
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MoexClient>();
            builder.Services.AddSingleton<DataManager>();

#if ANDROID
            builder.Services.AddSingleton<INotificationService, Platforms.Android.Services.AndroidNotificationService>();
#elif WINDOWS
            builder.Services.AddSingleton<INotificationService, Platforms.Windows.Services.WindowsNotificationService>();
#else
            builder.Services.AddSingleton<INotificationService, NullNotificationService>();
#endif

            builder.Services.AddSingleton<StockPollingWorker>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
