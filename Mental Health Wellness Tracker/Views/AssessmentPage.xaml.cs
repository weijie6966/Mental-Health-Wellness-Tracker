using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AssessmentPage : ContentPage
    {
        // The constructor now injects the ViewModel via Dependency Injection (DI)
        public AssessmentPage(AssessmentViewModel viewModel)
        {
            InitializeComponent();
            // Set the BindingContext to the injected ViewModel
            BindingContext = viewModel;
        }
    }
}