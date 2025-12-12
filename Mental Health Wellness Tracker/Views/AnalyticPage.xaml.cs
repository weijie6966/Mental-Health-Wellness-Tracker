using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AnalyticPage : ContentPage
    {
        private readonly AnalyticViewModel _viewModel;

        public AnalyticPage()
        {
            InitializeComponent();
            _viewModel = new AnalyticViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearingAsync();
        }
    }
}
