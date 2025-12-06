using Microsoft.Maui.Controls;
using System.Linq;

namespace Mental_Health_Wellness_Tracker
{
    public partial class AssessmentDetailPage : ContentPage
    {
        // This constructor MUST accept an AssessmentHistoryItem
        public AssessmentDetailPage(AssessmentHistoryItem item)
        {
            InitializeComponent();
            LoadData(item);
        }

        private void LoadData(AssessmentHistoryItem item)
        {
            if (item == null) return;

            LblDate.Text = item.Date.ToString("MMMM dd, yyyy - HH:mm");
            LblScore.Text = item.Score.ToString();
            LblStatus.Text = item.Status;

            // REBUILD CHART
            var scores = item.AnswerData;
            if (scores == null || scores.Count == 0) return;

            int low = scores.Count(s => s <= 1);
            int normal = scores.Count(s => s == 2);
            int high = scores.Count(s => s == 3);

            LblCountLow.Text = low.ToString();
            LblCountNormal.Text = normal.ToString();
            LblCountHigh.Text = high.ToString();

            // Set Bar Heights
            double m = 6.0;
            BarLow.HeightRequest = low * m;
            BarNormal.HeightRequest = normal * m;
            BarHigh.HeightRequest = high * m;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}