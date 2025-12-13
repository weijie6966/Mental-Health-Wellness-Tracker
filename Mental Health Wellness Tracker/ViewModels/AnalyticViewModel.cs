using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Views;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class AnalyticViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository = new AssessmentRepository();
        private string _userId;

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
        // FIX 1: Change type to the unified AssessmentResult model
        public List<AssessmentResult> History { get; private set; }

        // Commands
        public ICommand ViewHistoryDetailCommand { get; }
        public ICommand NavigateCommand { get; }

        public AnalyticViewModel()
        {
            _ = InitializeAsync();

            // FIX 2: Change parameter type in RelayCommand to AssessmentResult
            ViewHistoryDetailCommand = new RelayCommand(async param =>
                await OnViewHistoryDetailClicked(param as AssessmentResult));

            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        private async Task InitializeAsync()
        {
            try
            {
                _userId = await SecureStorage.GetAsync("user_id");
                await LoadHistoryAsync();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to load analytics: {ex.Message}", "OK");
            }
        }

        // --- Core Logic ---

        public void LoadStatistics()
        {
            var latestResult = History?.FirstOrDefault();
            if (latestResult == null)
            {
                ScoreDisplay = "-";
                StatusDisplay = "No data yet";
                Recommendation = "Take your first assessment to see your stats.";
                ResetBarData();
                NotifyStatisticProperties();
                return;
            }

            int score = latestResult.TotalScore;
            ScoreDisplay = score.ToString();
            StatusDisplay = GetStatusMessage(score);

            if (score <= 15) Recommendation = "Great spot! Keep practicing self-care.";
            else if (score <= 30) Recommendation = "Mild stress. Get enough sleep.";
            else if (score <= 45) Recommendation = "Moderate stress. Use the Diary feature.";
            else Recommendation = "High distress. Please reach out to a professional.";

            var scores = latestResult.AnswerData;

            // Reset state if no scores are present
            if (scores == null || scores.Count == 0)
            {
                ResetBarData();
            }
            else
            {
                int low = scores.Count(s => s <= 1);
                int normal = scores.Count(s => s == 2);
                int high = scores.Count(s => s == 3);

                CountLow = low.ToString();
                CountNormal = normal.ToString();
                CountHigh = high.ToString();

                const double heightPerItem = 28.0;
                const double maxHeight = 180.0;

                BarLowHeight = Math.Min(low * heightPerItem, maxHeight);
                BarNormalHeight = Math.Min(normal * heightPerItem, maxHeight);
                BarHighHeight = Math.Min(high * heightPerItem, maxHeight);
            }

            NotifyStatisticProperties();
        }

        private void ResetBarData()
        {
            BarLowHeight = 0;
            BarNormalHeight = 0;
            BarHighHeight = 0;
            CountLow = "0";
            CountNormal = "0";
            CountHigh = "0";
        }

        private void NotifyStatisticProperties()
        {
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

        public async Task LoadHistoryAsync()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                History = new List<AssessmentResult>();
                OnPropertyChanged(nameof(History));
                return;
            }

            var results = await _repository.GetAssessmentHistoryAsync(_userId);
            History = results
                ?.OrderByDescending(x => x.DateTaken)
                .ToList()
                ?? new List<AssessmentResult>();

            OnPropertyChanged(nameof(History));
            LoadStatistics();
        }

        private async Task OnViewHistoryDetailClicked(AssessmentResult result)
        {
            if (result != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetailPage(result));
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
        public async Task OnAppearingAsync()
        {
            await LoadHistoryAsync();
        }

        private string GetStatusMessage(int score)
        {
            if (score <= 15) return "Minimal Stress";
            if (score <= 30) return "Mild Stress";
            if (score <= 45) return "Moderate Stress";
            return "High Stress/Severe Distress";
        }
    }
}