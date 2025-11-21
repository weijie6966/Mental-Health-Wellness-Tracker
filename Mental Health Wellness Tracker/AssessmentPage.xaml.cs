using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;
using System.ComponentModel;

namespace Mental_Health_Wellness_Tracker
{
    // Model for a single question
    // This class is dependent on AssessmentPage.xaml and its properties (Text, Answers)
    // Upgrade the Question class to support property notifications (INotifyPropertyChanged)
    // This way, the interface will be notified when SelectedScore changes.
    public class Question : INotifyPropertyChanged
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Answers { get; set; } = new List<string>();
        // You could add properties for SelectedAnswer, Score, etc.

        // We'll add a hidden attribute here to store the question ID in the database,
        // making it easier to save the answer later.
        public int DbId { get; set; }

        // Is the storage question itself subject to reverse scoring
        public bool IsReversed { get; set; }
        public int MaxScore { get; set; } // 3 for Rosenberg, 4 for PSS

        // The score selected by the user (default is -1 to indicate no selection).
        private int _selectedScore = -1;
        public int SelectedScore
        {
            get => _selectedScore;
            set
            {
                if (_selectedScore != value)
                {
                    _selectedScore = value;
                    OnPropertyChanged(nameof(SelectedScore));
                    OnPropertyChanged(nameof(SelectedScoreText));
                }
            }
        }

        public string SelectedScoreText => SelectedScore == -1 ? "Tap an option" : $"Selected: {SelectedScore}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public partial class AssessmentPage : ContentPage
    {
        // Define private fields to store database repositories
        private readonly IAssessmentRepository _repository;
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        public AssessmentPage(IAssessmentRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            // Calling asynchronous loading method
            LoadQuestions();

            // Required to set the context for the CollectionView to find the 'Questions' property
            this.BindingContext = this;
        }

        private async void LoadQuestions()
        {
            // Fetch questions from the database for the "Rosenberg" test type
            // In the future, you can pass in either "PSS" or "Rosenberg" based on the entry point the user clicks.
            var dbQuestions = await _repository.GetQuestionsByTestTypeAsync("Rosenberg");

            // Define standardized options (the Rosenberg scale typically has these four).
            var standardAnswers = new List<string>
            {
                "Strongly Disagree",
                "Disagree",
                "Agree",
                "Strongly Agree"
            };

            // Data transformation: Convert the database model (AssessmentQuestion) into a user interface model (Question).
            // This allows us to keep the UI logic separate from the database schema.
            var uiQuestions = new ObservableCollection<Question>();
            foreach (var q in dbQuestions)
            {
                uiQuestions.Add(new Question
                {
                    DbId = q.Id,                // Store the database ID for later use
                    Text = q.QuestionText,      // Set the question text
                    Answers = standardAnswers,   // Use the standardized answer options
                    IsReversed = q.IsReversed,  // Set reverse scoring flag
                    MaxScore = q.MaxScore       // Set maximum score for the question
                });
            }

            // Update user interface
            Questions = uiQuestions;
            QuestionsCollection.ItemsSource = Questions;
        }

        // Click on the processing option
        private void OnAnswerClicked(object sender, EventArgs e)
        {
            // Here's a slightly clever trick: find the Question object that the Button is bound to.
            var button = sender as Button;
            var question = button?.BindingContext as Question;

            if (question != null && button != null)
            {
                // Get the score represented by the button (0, 1, 2, 3)
                // Note: For simplicity, we didn't use Binding to pass the CommandParameter in the XAML,
                // Instead, using Text or Grid.Column would be more complex.
                // The simplest correction:
                // It's difficult to directly pass the parameters 0, 1, 2, 3 in XAML.
                // Here we use a workaround: determine the score by the button's index in the Answers list.

                string answerText = button.Text;
                int index = question.Answers.IndexOf(answerText);

                if (index != -1)
                {
                    question.SelectedScore = index;
                }
            }
        }

        // Submit and save
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            // Validation: Ensure all questions have been answered
            if (Questions.Any(q => q.SelectedScore == -1))
            {
                await DisplayAlert("Incomplete", "Please answer all questions before submitting.", "OK");
                return;
            }

            // Calculate the total score
            int totalScore = 0;
            //var detailedAnswers = new List<AnswerDetail>(); // Preparing to save detailed answers

            foreach (var q in Questions)
            {
                int finalPoints = q.SelectedScore;

                // Handling reverse scoring logic
                if (q.IsReversed)
                {
                    finalPoints = q.MaxScore - q.SelectedScore;
                }

                totalScore += finalPoints;
            }

            // Evaluation of generated results
            string resultText = "Normal Self-Esteem";
            if (totalScore < 15) resultText = "Low Self-Esteem";
            else if (totalScore <= 25) resultText = "Normal Self-Esteem";
            else resultText = "High Self-Esteem";

            // Build database objects
            var result = new AssessmentResult
            {
                UserId = await SecureStorage.GetAsync("user_id") ?? "unknown_user", // Get the ID of the currently logged-in user
                TestType = "Rosenberg",
                DateTaken = DateTime.Now,
                TotalScore = totalScore,
                CalculatedResult = resultText,
                IsSynced = false,
                // AnswersJson = ... (If you want to store detailed JSON, you can serialize detailedAnswers here)
            };

            // Save to the database
            bool isSaved = await _repository.SaveAssessmentResultAsync(result);

            if (isSaved)
            {
                await DisplayAlert("Result", $"Your Score: {totalScore}\nResult: {resultText}", "OK");
                // Navigate back or to another page as needed
                //await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to save the result.", "OK");
            }
        }

        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage(_repository));
            }
            else if (destination == "Diary")
            {
                await Navigation.PushAsync(new WriteDiaryPage());
            }
            else if (destination == "Stats")
            {
                // Get warehouse service
                var repo = IPlatformApplication.Current.Services.GetService<Services.IAssessmentRepository>();
                // Navigate to the analytics page
                await Navigation.PushAsync(new AnalyticPage(repo));
            }
            else if (destination == "Profile")
            {
                await Navigation.PushAsync(new ProfilePage());
            }
        }
    }
}