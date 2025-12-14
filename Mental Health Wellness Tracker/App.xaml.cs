using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Views;

namespace Mental_Health_Wellness_Tracker
{
    public partial class App : Application
    {
        private readonly NotificationService _notificationService;

        public App(NotificationService notificationService)
        {
            InitializeComponent();

            _notificationService = notificationService;

            // Start the app on the login page without relying on DI
            MainPage startPage = new MainPage();
            MainPage = new NavigationPage(startPage);

            _ = InitializeNotificationsAsync();
        }

        private async Task InitializeNotificationsAsync()
        {
            await _notificationService.InitializeAsync();
        }
    }
}
