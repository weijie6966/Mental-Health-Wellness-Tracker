using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker
{
    public partial class AssessmentDetailPage : ContentPage
    {
        // This constructor MUST accept an AssessmentHistoryItem
        public AssessmentDetailPage(AssessmentHistoryItem item)
        {
            InitializeComponent();

            // Set the BindingContext to the new ViewModel, passing the required data.
            this.BindingContext = new AssessmentDetailViewModel(item);
        }

        // All logic is handled by AssessmentDetailViewModel.
    }
}