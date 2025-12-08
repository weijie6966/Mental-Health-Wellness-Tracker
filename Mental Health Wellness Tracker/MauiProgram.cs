using Mental_Health_Wellness_Tracker.ViewModels;
using Mental_Health_Wellness_Tracker;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

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
                fonts.AddFont("BrushFont.ttf", "BrushFont");
            })
            .RegisterPagesAndViewModels();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    // Extension method to register all Pages and ViewModels
    public static MauiAppBuilder RegisterPagesAndViewModels(this MauiAppBuilder builder)
    {
        // Register the ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<ForgotPasswordViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<WriteDiaryViewModel>();
        builder.Services.AddTransient<AssessmentViewModel>();
        builder.Services.AddTransient<AnalyticViewModel>();      // <-- CONVERTED
        builder.Services.AddTransient<AssessmentDetailViewModel>(); // <-- CONVERTED

        // Register other existing Pages (for use in navigation commands)
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SignUpPage>();
        builder.Services.AddTransient<SignUpSuccessPage>();
        builder.Services.AddTransient<ForgotPasswordPage>();
        builder.Services.AddTransient<WriteDiaryPage>();
        builder.Services.AddTransient<CommunityPage>();
        builder.Services.AddTransient<AssessmentPage>();
        builder.Services.AddTransient<AnalyticPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ProfilePictureViewPage>();
        builder.Services.AddTransient<AssessmentDetailPage>();

        return builder;
    }
}