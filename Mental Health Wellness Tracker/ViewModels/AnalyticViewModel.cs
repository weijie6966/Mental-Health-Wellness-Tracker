using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Views;
using Microsoft.Extensions.DependencyInjection; // ADDED: Needed for GetService<T>()

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
        // FIX 1: Change type to the unified AssessmentResult model
        public List<AssessmentResult> History { get; private set; }

        // Commands
        public ICommand ViewHistoryDetailCommand { get; }
        public ICommand NavigateCommand { get; }

        public AnalyticViewModel()
        {
            // Initial data load in constructor
            // NOTE: Static data access requires careful management of state and synchronization
            LoadStatistics();
            LoadHistory();

            // FIX 2: Change parameter type in RelayCommand to AssessmentResult
            ViewHistoryDetailCommand = new RelayCommand(async param =>
                await OnViewHistoryDetailClicked(param as AssessmentResult));

            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // --- Core Logic ---

        public void LoadStatistics()
        {
            // ASSUMPTION: You are now using static properties on AssessmentResult for current state
            int score = AssessmentState.CurrentScore;
            ScoreDisplay = score.ToString();
            StatusDisplay = AssessmentState.GetStatusMessage(score);

            if (score <= 15) Recommendation = "Great spot! Keep practicing self-care.";
            else if (score <= 30) Recommendation = "Mild stress. Get enough sleep.";
            else if (score <= 45) Recommendation = "Moderate stress. Use the Diary feature.";
            else Recommendation = "High distress. Please reach out to a professional.";

            // ASSUMPTION: QuestionScores property exists on AssessmentResult and returns List<int>
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
                BarNormalHeight = normal * m * 150; // *150 added for visual scaling consistency
                BarHighHeight = high * m * 150;     // *150 added for visual scaling consistency
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
            // ASSUMPTION: AssessmentResult.History property exists and returns List<AssessmentResult>
            // FIX 3: Property access is corrected (History is now List<AssessmentResult>)
            // FIX 4: Use DateTaken property on the AssessmentResult model
            History = AssessmentState.History.OrderByDescending(x => x.DateTaken).ToList();
            OnPropertyChanged(nameof(History));
        }

        private async Task OnViewHistoryDetailClicked(AssessmentResult result)
        {
            if (result != null)
            {
                // Retrieve the service provider
                IServiceProvider services = Application.Current?.Handler?.MauiContext?.Services;

                if (services == null) return;

                // FIX 5: Use DI to create the page, ensuring AssessmentDetailViewModel is injected
                var nextPage = services.GetService<AssessmentDetailPage>();

                // You will need to manually set the BindingContext here, or modify the 
                // AssessmentDetailPage constructor to accept the result data as well.
                // Assuming AssessmentDetailPage has a method to initialize with data:
                // nextPage.InitializeWithData(result); 

                // NOTE: Since you are using a new AssessmentDetailPage(historyItem) 
                // pattern, we'll revert to that for simplicity, but acknowledge it 
                // means AssessmentDetailPage must manually create its ViewModel.

                await Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetailPage(result));
            }
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Stats") return;

            // FIX 6: Use the service provider to retrieve the page, resolving the DI issue
            IServiceProvider services = Application.Current?.Handler?.MauiContext?.Services;
            if (services == null) return;

            Page nextPage = destination switch
            {
                // Use GetService<T>() for all pages requiring DI
                "Community" => services.GetService<CommunityPage>(),
                "List" => services.GetService<AssessmentPage>(),
                "Diary" => services.GetService<WriteDiaryPage>(),
                "Profile" => services.GetService<ProfilePage>(),
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