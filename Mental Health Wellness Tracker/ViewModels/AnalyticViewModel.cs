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
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to load analytics: {ex.Message}", "OK");
            }
        }

        // --- Core Logic ---

        public async Task LoadStatisticsAsync()
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

            if (score < 15) Recommendation = "Self-esteem is low. Consider journaling what you appreciate about yourself.";
            else if (score <= 25) Recommendation = "You are in the normal range—keep reinforcing healthy self-talk.";
            else Recommendation = "High self-esteem detected. Maintain balance with mindful reflection.";

            var scores = await ScoreAnswersAsync(latestResult);

            // Reset state if no scores are present
            if (scores == null || scores.Count == 0)
            {
                ResetBarData();
            }
            else
            {
                var (low, normal, high) = CalculateSymptomFrequency(scores);

                CountLow = low.ToString();
                CountNormal = normal.ToString();
                CountHigh = high.ToString();

                // Cap bar height to keep the chart labels visible
                const double heightPerItem = 24.0;
                const double maxHeight = 120.0;

                BarLowHeight = Math.Min(low * heightPerItem, maxHeight);
                BarNormalHeight = Math.Min(normal * heightPerItem, maxHeight);
                BarHighHeight = Math.Min(high * heightPerItem, maxHeight);
            }

            NotifyStatisticProperties();
        }

        private async Task<List<int>> ScoreAnswersAsync(AssessmentResult result)
        {
            if (result?.AnswerData == null || result.AnswerData.Count == 0)
            {
                return new List<int>();
            }

            var questions = await _repository.GetQuestionsByTestTypeAsync(result.TestType) ?? new List<AssessmentQuestion>();
            var orderedQuestions = questions.OrderBy(q => q.OrderIndex).ToList();

            var scored = new List<int>();
            var count = Math.Min(result.AnswerData.Count, orderedQuestions.Count);

            const int rosenbergMax = 3;

            for (int i = 0; i < count; i++)
            {
                var question = orderedQuestions[i];
                var rawSelection = result.AnswerData[i];
                var normalized = Math.Clamp(rawSelection, 0, rosenbergMax);

                var scoredValue = question.IsReversed
                    ? Math.Max(0, rosenbergMax - normalized)
                    : normalized;

                scored.Add(scoredValue);
            }

            return scored;
        }

        private (int low, int normal, int high) CalculateSymptomFrequency(List<int> scores)
        {
            int low = 0, normal = 0, high = 0;

            foreach (var score in scores)
            {
                if (score == 0) low++;
                else if (score == 1) normal++;
                else high++;
            }

            return (low, normal, high);
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
            await LoadStatisticsAsync();
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
            if (score < 15) return "Low Self-Esteem";
            if (score <= 25) return "Normal Self-Esteem";
            return "High Self-Esteem";
        }
    }
}