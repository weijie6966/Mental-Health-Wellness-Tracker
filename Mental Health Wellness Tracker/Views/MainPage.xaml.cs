using Mental_Health_Wellness_Tracker.ViewModels;
using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Since the BindingContext is set in MainPage.xaml using <vm:MainViewModel />,
            // this line is optional but harmless.
            // this.BindingContext = new MainViewModel(); 
        }

        // All previous event handlers and fields are now removed.
    }
}