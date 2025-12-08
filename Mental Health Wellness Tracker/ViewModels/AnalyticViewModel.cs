using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Models;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class AnalyticViewModel : ViewModelBase
    {
        // Properties bound to the View (Current Statistics)
        public string ScoreDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public string Recommendation { get; private set; }

        // Bar Chart Data (Heights and Counts)
        public double BarLowHeight { get; private set; }
        public double BarNormalHeight { get; private set; }
        public double BarHighHeight { get; private set; }
        public string CountLow { get; private set; }
        public string CountNormal { get; private set; }
        public string CountHigh { get; private set; }

        // History List
        public List<AssessmentHistoryItem> History { get; private set; }

        // Commands
        public ICommand ViewHistoryDetailCommand { get; }
        public ICommand NavigateCommand { get; }

        public AnalyticViewModel()
        {
            // Initial data load in constructor
            LoadStatistics();
            LoadHistory();

            ViewHistoryDetailCommand = new RelayCommand(async param => await OnViewHistoryDetailClicked(param as AssessmentHistoryItem));
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // --- Core Logic (Moved from AnalyticPage.xaml.cs) ---

        public void LoadStatistics()
        {
            int score = AssessmentState.CurrentScore;
            ScoreDisplay = score.ToString();
            StatusDisplay = AssessmentState.GetStatusMessage(score);

            if (score <= 15) Recommendation = "Great spot! Keep practicing self-care.";
            else if (score <= 30) Recommendation = "Mild stress. Get enough sleep.";
            else if (score <= 45) Recommendation = "Moderate stress. Use the Diary feature.";
            else Recommendation = "High distress. Please reach out to a professional.";

            var scores = AssessmentState.QuestionScores;

            // Reset state if no scores are present
            if (scores == null || scores.Count == 0)
            {
                BarLowHeight = 0; BarNormalHeight = 0; BarHighHeight = 0;
                CountLow = "0"; CountNormal = "0"; CountHigh = "0";
            }
            else
            {
                int low = scores.Count(s => s <= 1);
                int normal = scores.Count(s => s == 2);
                int high = scores.Count(s => s == 3);

                CountLow = low.ToString();
                CountNormal = normal.ToString();
                CountHigh = high.ToString();

                double m = 6.0;
                BarLowHeight = low * m;
                BarNormalHeight = normal * m;
                BarHighHeight = high * m;
            }

            // Notify all relevant properties to update the UI
            OnPropertyChanged(nameof(ScoreDisplay));
            OnPropertyChanged(nameof(StatusDisplay));
            OnPropertyChanged(nameof(Recommendation));
            OnPropertyChanged(nameof(CountLow));
            OnPropertyChanged(nameof(CountNormal));
            OnPropertyChanged(nameof(CountHigh));
            OnPropertyChanged(nameof(BarLowHeight));
            OnPropertyChanged(nameof(BarNormalHeight));
            OnPropertyChanged(nameof(BarHighHeight));
        }

        public void LoadHistory()
        {
            // Order history by descending date
            History = AssessmentState.History.OrderByDescending(x => x.Date).ToList();
            OnPropertyChanged(nameof(History));
        }

        private async Task OnViewHistoryDetailClicked(AssessmentHistoryItem historyItem)
        {
            if (historyItem != null)
            {
                // Instantiate the detail page passing the model, and the page will handle setting the detail ViewModel
                await Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetailPage(historyItem));
            }
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Stats") return;

            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "List" => new AssessmentPage(),
                "Diary" => new WriteDiaryPage(),
                "Profile" => new ProfilePage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }

        // Expose public method to call from page OnAppearing
        public void OnAppearing()
        {
            LoadStatistics();
            LoadHistory();
        }
    }
}