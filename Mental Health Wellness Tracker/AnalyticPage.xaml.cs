using Microsoft.Maui.Controls;
using System.Linq;

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
            LoadStatistics();
            LoadHistory();
        }

        private void LoadStatistics()
        {
            // Basic Score
            int score = AssessmentState.CurrentScore;
            LblScore.Text = score.ToString();
            LblStatus.Text = AssessmentState.GetStatusMessage(score);

            if (score <= 15) LblRecommendation.Text = "Great spot! Keep practicing self-care.";
            else if (score <= 30) LblRecommendation.Text = "Mild stress. Get enough sleep.";
            else if (score <= 45) LblRecommendation.Text = "Moderate stress. Use the Diary feature.";
            else LblRecommendation.Text = "High distress. Please reach out to a professional.";

            // Bar Chart Logic
            var scores = AssessmentState.QuestionScores;

            if (scores == null || scores.Count == 0)
            {
                BarLow.HeightRequest = 0; BarNormal.HeightRequest = 0; BarHigh.HeightRequest = 0;
                LblCountLow.Text = "0"; LblCountNormal.Text = "0"; LblCountHigh.Text = "0";
                return;
            }

            int low = scores.Count(s => s <= 1);
            int normal = scores.Count(s => s == 2);
            int high = scores.Count(s => s == 3);

            LblCountLow.Text = low.ToString();
            LblCountNormal.Text = normal.ToString();
            LblCountHigh.Text = high.ToString();

            // Multiplier for 20 questions
            double m = 6.0;
            BarLow.HeightRequest = low * m;
            BarNormal.HeightRequest = normal * m;
            BarHigh.HeightRequest = high * m;
        }

        private void LoadHistory()
        {
            var history = AssessmentState.History;
            if (history != null && history.Count > 0)
            {
                HistoryCollectionView.ItemsSource = history.OrderByDescending(x => x.Date).ToList();
            }
        }

        private async void OnViewHistoryClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var historyItem = button.BindingContext as AssessmentHistoryItem;

            if (historyItem != null)
            {
                await Navigation.PushAsync(new AssessmentDetailPage(historyItem));
            }
        }

        // --- UPDATED NAVIGATION LOGIC ---
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string d = ((Button)sender).AutomationId;
            if (d == "Community") await Navigation.PushAsync(new CommunityPage());
            else if (d == "List") await Navigation.PushAsync(new AssessmentPage());
            else if (d == "Diary") await Navigation.PushAsync(new WriteDiaryPage());
            else if (d == "Profile") await Navigation.PushAsync(new ProfilePage());
        }
    }
}