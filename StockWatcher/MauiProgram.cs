using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
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

            // builder.Services.AddTransientPopup<StockSelector, StockSelector>();
            builder.Services.AddSingleton<StockPollingWorker>();


#if ANDROID
        builder.Services.AddSingleton<INotificationService, Platforms.Android.Services.AndroidNotificationService>();
#elif WINDOWS
        builder.Services.AddSingleton<INotificationService, Platforms.Windows.Services.WindowsNotificationService>();
#endif


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
