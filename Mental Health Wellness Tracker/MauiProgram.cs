using Mental_Health_Wellness_Tracker;

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

        return builder.Build();
    }
}