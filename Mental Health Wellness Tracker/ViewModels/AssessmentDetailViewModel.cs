using System;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Models;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class AssessmentDetailViewModel : ViewModelBase
    {
        // FIX: Change the private field type from AssessmentHistoryItem to AssessmentResult
        private AssessmentResult _item;

        // Properties bound to the View
        public string DateDisplay { get; private set; }
        public string ScoreDisplay { get; private set; }
        public string StatusDisplay { get; private set; }

        // Bar Chart Data (Heights and Counts)
        public double BarLowHeight { get; private set; }
        public double BarNormalHeight { get; private set; }
        public double BarHighHeight { get; private set; }
        public string CountLow { get; private set; }
        public string CountNormal { get; private set; }
        public string CountHigh { get; private set; }

        // Commands
        public ICommand BackCommand { get; }

        public AssessmentDetailViewModel(AssessmentResult item)
        {
            _item = item; // This is now a valid assignment
            LoadData(item);

            // NOTE: Ensure RelayCommand class is accessible (e.g., in a Commands folder or this namespace)
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());
        }

        private void LoadData(AssessmentResult item)
        {
            // This part is already correct, using item.AnswerData (the computed property)
            // and the correct consolidated properties (DateTaken, TotalScore, CalculatedResult).
            if (item == null || item.AnswerData == null || item.AnswerData.Count == 0) return;

            DateDisplay = item.DateTaken.ToString("MMMM dd, yyyy - HH:mm");
            ScoreDisplay = item.TotalScore.ToString();
            StatusDisplay = item.CalculatedResult;

            var scores = item.AnswerData;

            int low = scores.Count(s => s <= 1);
            int normal = scores.Count(s => s == 2);
            int high = scores.Count(s => s == 3);

            CountLow = low.ToString();
            CountNormal = normal.ToString();
            CountHigh = high.ToString();

            // Multiplier for bar height (from AnalyticPage.xaml.cs: m = 6.0)
            double m = 6.0;
            BarLowHeight = low * m;
            BarNormalHeight = normal * m;
            BarHighHeight = high * m;

            // Manually notify all properties (important for changes made post-initialization)
            OnPropertyChanged(nameof(DateDisplay));
            OnPropertyChanged(nameof(ScoreDisplay));
            OnPropertyChanged(nameof(StatusDisplay));
            OnPropertyChanged(nameof(CountLow));
            OnPropertyChanged(nameof(CountNormal));
            OnPropertyChanged(nameof(CountHigh));
            OnPropertyChanged(nameof(BarLowHeight));
            OnPropertyChanged(nameof(BarNormalHeight));
            OnPropertyChanged(nameof(BarHighHeight));
        }
    }
}