using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AssessmentPage : ContentPage
    {
        public AssessmentPage()
        {
            InitializeComponent();
            BindingContext = new AssessmentViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            AssessmentContent.Opacity = 0;
            AssessmentContent.TranslationY = 20;

            await Task.WhenAll(
                AssessmentContent.FadeTo(1, 250, Easing.CubicOut),
                AssessmentContent.TranslateTo(0, 0, 250, Easing.CubicOut));
        }
    }
}
