using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AnalyticPage : ContentPage
    {
        // Add a constructor that accepts the ViewModel
        // MAUI's DI container automatically passes the required ViewModel instance here.
        public AnalyticPage(AnalyticViewModel viewModel)
        {
            InitializeComponent();

            // Set the BindingContext here, after the ViewModel is injected.
            this.BindingContext = viewModel;
        }
    }
}