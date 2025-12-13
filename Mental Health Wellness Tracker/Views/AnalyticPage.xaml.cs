using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;
using System.Threading.Tasks;

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
            AnalyticContent.Opacity = 0;
            AnalyticContent.TranslationY = 20;

            await Task.WhenAll(
                AnalyticContent.FadeTo(1, 250, Easing.CubicOut),
                AnalyticContent.TranslateTo(0, 0, 250, Easing.CubicOut));

            await _viewModel.OnAppearingAsync();
        }
    }
}
