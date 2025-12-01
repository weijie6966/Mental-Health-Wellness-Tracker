using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker
{
    public partial class App : Application
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();

            // Wrap MainPage in a NavigationPage so we can push/pop screens
            MainPage = new NavigationPage(mainPage);
        }
    }
}
