using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Plugin.LocalNotification;
using Mental_Health_Wellness_Tracker.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<Mental_Health_Wellness_Tracker.App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("BrushFont.ttf", "BrushFont");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Comfortaa-Regular.ttf", "ComfortaaRegular");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<NotificationService>();

        return builder.Build();
    }

    
}