using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Views;

namespace Mental_Health_Wellness_Tracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Start the app on the login page without relying on DI
            MainPage startPage = new MainPage();
            MainPage = new NavigationPage(startPage);
        }
    }
}