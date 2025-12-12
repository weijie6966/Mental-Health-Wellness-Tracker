using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Views;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;
using Microsoft.Maui.Storage;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // FIX: Class uses the base AssessmentQuestion model directly
    public class AssessmentViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _assessmentRepository = new AssessmentRepository();

        private const string DefaultTestType = "PSS";

        // FIX: ObservableCollection now holds the base AssessmentQuestion model
        public ObservableCollection<AssessmentQuestion> Questions { get; set; } = new ObservableCollection<AssessmentQuestion>();
        public AssessmentQuestion CurrentQuestion { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public string QuestionCounterDisplay => $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";
        public bool IsNotFirstQuestion => CurrentQuestionIndex > 0;
        public bool IsNotLastQuestion => CurrentQuestionIndex < Questions.Count - 1;
        public bool IsSubmitVisible => CurrentQuestionIndex == Questions.Count - 1;

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SelectOptionCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand NavigateCommand { get; }

        public AssessmentViewModel()
        {
            NextCommand = new RelayCommand(_ => MoveNext(), _ => CanMoveNext());
            PreviousCommand = new RelayCommand(_ => MovePrevious(), _ => CanMovePrevious());
            SelectOptionCommand = new RelayCommand<int>(score => SelectOption(score));
            SubmitCommand = new RelayCommand(async _ => await SubmitAssessment(), _ => CanSubmit());
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadQuestionsAsync();
            CurrentQuestion = Questions.FirstOrDefault();
            UpdateQuestionState();
        }

        private async Task LoadQuestionsAsync()
        {
            Questions.Clear();

            var fetched = await _assessmentRepository.GetQuestionsByTestTypeAsync(DefaultTestType);

            if (fetched != null && fetched.Count > 0)
            {
                foreach (var q in fetched)
                {
                    q.SelectedScore = null;
                    Questions.Add(q);
                }
            }
            else
            {
                // Fallback to a minimal built-in set if the repository is empty
                Questions.Add(new AssessmentQuestion { Id = 1, QuestionText = "I have been feeling down, depressed, or hopeless." });
                Questions.Add(new AssessmentQuestion { Id = 2, QuestionText = "I have had little interest or pleasure in doing things." });
                Questions.Add(new AssessmentQuestion { Id = 3, QuestionText = "I have had trouble falling or staying asleep, or sleeping too much." });
                Questions.Add(new AssessmentQuestion { Id = 4, QuestionText = "I have been feeling tired or having little energy." });
                Questions.Add(new AssessmentQuestion { Id = 5, QuestionText = "I have had poor appetite or overeating." });
            }

            CurrentQuestionIndex = 0;
        }

        private void SelectOption(int score)
        {
            if (CurrentQuestion != null)
            {
                // FIX: Uses the SelectedScore property directly from the model (now available)
                CurrentQuestion.SelectedScore = score;
                if (CanMoveNext())
                {
                    MoveNext();
                }
                else
                {
                    UpdateQuestionState();
                }
            }
        }

        private void MoveNext()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
            UpdateQuestionState();
        }

        private void UpdateQuestionState()
        {
            ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(QuestionCounterDisplay));
            OnPropertyChanged(nameof(IsNotFirstQuestion));
            OnPropertyChanged(nameof(IsNotLastQuestion));
            OnPropertyChanged(nameof(IsSubmitVisible));
        }

        private bool CanMoveNext() => CurrentQuestionIndex < Questions.Count - 1 && CurrentQuestion.SelectedScore.HasValue;
        private void MovePrevious()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
            UpdateQuestionState();
        }
        private bool CanMovePrevious() => CurrentQuestionIndex > 0;
        private bool CanSubmit() => CurrentQuestionIndex == Questions.Count - 1 && Questions.All(q => q.SelectedScore.HasValue);


        private async Task SubmitAssessment()
        {
            if (!CanSubmit()) return;

            int totalScore = Questions.Sum(q => q.SelectedScore.GetValueOrDefault());

            try
            {
                string userId = await SecureStorage.GetAsync("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User not authenticated for assessment submission.");
                }

                var userEmail = await SecureStorage.GetAsync("user_email");
                var username = Preferences.Get("UsernameKey", "User");
                var answers = Questions.Select(q => q.SelectedScore ?? 0).ToList();

                // FIX: Uses properties from your AssessmentResult.cs (TotalScore, DateTaken, CalculatedResult)
                var result = new AssessmentResult
                {
                    UserId = userId,
                    UserEmail = userEmail,
                    Username = username,
                    TestType = DefaultTestType,
                    TotalScore = totalScore, // Correct property
                    DateTaken = DateTime.UtcNow, // Correct property
                    CalculatedResult = GetCalculatedResult(totalScore), // Correct property
                    AnswerData = answers
                };

                await _assessmentRepository.SaveAssessmentResultAsync(result);

                // Navigates to AssessmentDetailPage, passing the result object
                // Inside SubmitAssessment() method:
                await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetailPage(result));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", $"Failed to submit assessment: {ex.Message}", "OK");
            }
        }

        // FIX: Method name changed to match the property name
        private string GetCalculatedResult(int score)
        {
            if (score <= 4) return "Minimal Depression";
            if (score <= 9) return "Mild Depression";
            if (score <= 14) return "Moderate Depression";
            if (score <= 19) return "Moderately Severe Depression";
            return "Severe Depression";
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;
            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "Diary" => new WriteDiaryPage(),
                "Stats" => new AnalyticPage(),
                "Profile" => new ProfilePage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}