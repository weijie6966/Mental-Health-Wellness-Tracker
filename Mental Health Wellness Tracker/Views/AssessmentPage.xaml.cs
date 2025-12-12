using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AssessmentPage : ContentPage
    {
        public AssessmentPage()
        {
            InitializeComponent();
            BindingContext = new AssessmentViewModel();
        }
    }
}
