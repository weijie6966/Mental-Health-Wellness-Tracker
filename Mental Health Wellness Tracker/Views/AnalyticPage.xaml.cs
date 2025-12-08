using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker
{
    public partial class AnalyticPage : ContentPage
    {
        public AnalyticPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Trigger the ViewModel's loading logic when the page becomes visible
            if (this.BindingContext is AnalyticViewModel viewModel)
            {
                viewModel.OnAppearing();
            }
        }

        // All navigation and logic are handled by AnalyticViewModel.
    }
}