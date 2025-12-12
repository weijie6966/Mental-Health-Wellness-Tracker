using Mental_Health_Wellness_Tracker.ViewModels;
using Microsoft.Maui.Controls;

// Assuming your pages are in the Views namespace
namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
}
