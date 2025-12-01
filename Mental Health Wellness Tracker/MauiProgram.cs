using Mental_Health_Wellness_Tracker;
using Mental_Health_Wellness_Tracker.Services;
using Microsoft.Extensions.Logging;

namespace Mental_Health_Wellness_Tracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                // Add your custom brush font here:
                fonts.AddFont("BrushFont.ttf", "BrushFont");
            });

        // Register AuthService for dependency injection
        builder.Services.AddSingleton<IAuthService, AuthService>();
        // Register AssessmentRepository for dependency injection
        builder.Services.AddSingleton<IAssessmentRepository, AssessmentRepository>();
        // Register MainPage & App for dependency injection
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<App>();

        return builder.Build();
    }
}