using Mental_Health_Wellness_Tracker.ViewModels;
using Mental_Health_Wellness_Tracker.Services; // <-- REQUIRED for IAuthService/IAssessmentRepository
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.DependencyInjection; // Essential for AddTransient/AddSingleton
using Mental_Health_Wellness_Tracker.Views;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<Mental_Health_Wellness_Tracker.App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("BrushFont.ttf", "BrushFont");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Comfortaa-Regular.ttf", "ComfortaaRegular");
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
        // -------------------------
        // 1. REGISTER SERVICES (Business Logic/Data Access)
        // -------------------------

        // FIX 1: Authentication Service (Used by Main/SignUp ViewModels)
        // Use Singleton if the AuthClient/Firebase client should exist for the app's lifetime.
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // FIX 2: Assessment/Diary Repository (Used by Analytic/Community/WriteDiary ViewModels)
        // Use Singleton if the repository manages a single database connection/instance.
        builder.Services.AddSingleton<IAssessmentRepository, AssessmentRepository>();

        // You would also register IStorageService here if you implemented it for images.
        // builder.Services.AddSingleton<IStorageService, FirebaseStorageService>(); 

        // -------------------------
        // 2. REGISTER VIEWMODELS (All are Transient as they are tied to a page lifecycle)
        // -------------------------
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SignUpViewModel>();
        builder.Services.AddTransient<ForgotPasswordViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<WriteDiaryViewModel>();
        builder.Services.AddTransient<AssessmentViewModel>();
        builder.Services.AddTransient<AnalyticViewModel>();
        builder.Services.AddTransient<AssessmentDetailViewModel>();
        builder.Services.AddTransient<ContactUsViewModel>();

        // -------------------------
        // 3. REGISTER PAGES (All are Transient as they often require unique instances)
        // -------------------------
        // FIX 3: Registering App itself for DI fix in App.xaml.cs constructor
        builder.Services.AddSingleton<Mental_Health_Wellness_Tracker.App>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.SignUpPage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.SignUpSuccessPage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.ForgotPasswordPage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.WriteDiaryPage>();

        // Note: The fully qualified names were used to resolve ambiguity, which is fine.
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.Views.CommunityPage>();
        builder.Services.AddTransient<AssessmentPage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.Views.AnalyticPage>();

        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.ProfilePage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.ProfilePictureViewPage>();
        builder.Services.AddTransient<Mental_Health_Wellness_Tracker.AssessmentDetailPage>();
        builder.Services.AddTransient<ContactUsPage>();


        return builder;
    }
}