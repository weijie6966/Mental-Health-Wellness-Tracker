using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mental_Health_Wellness_Tracker
{
    // --- CLASS DEFINITIONS ---

    // We define 'Question' here. It should NOT be in other files.
    public class Question : INotifyPropertyChanged
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Answers { get; set; } = new List<string>();
        public int SelectedScore { get; set; } = 0;

        private Color _c0 = Colors.DeepSkyBlue;
        private Color _c1 = Colors.DeepSkyBlue;
        private Color _c2 = Colors.DeepSkyBlue;
        private Color _c3 = Colors.DeepSkyBlue;

        public Color Color0 { get => _c0; set { _c0 = value; OnPropertyChanged(); } }
        public Color Color1 { get => _c1; set { _c1 = value; OnPropertyChanged(); } }
        public Color Color2 { get => _c2; set { _c2 = value; OnPropertyChanged(); } }
        public Color Color3 { get => _c3; set { _c3 = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // --- PAGE LOGIC ---

    public partial class AssessmentPage : ContentPage
    {
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        public AssessmentPage()
        {
            InitializeComponent();
            LoadQuestions();
            this.BindingContext = this;
        }

        private void LoadQuestions()
        {
            var standardAnswers = new List<string> { "Not at all", "Several days", "More than half", "Nearly every day" };

            Questions = new ObservableCollection<Question>
            {
                new Question { Text = "1. Little interest or pleasure in doing things?", Answers = standardAnswers },
                new Question { Text = "2. Feeling down, depressed, or hopeless?", Answers = standardAnswers },
                new Question { Text = "3. Trouble falling or staying asleep, or sleeping too much?", Answers = standardAnswers },
                new Question { Text = "4. Feeling tired or having little energy?", Answers = standardAnswers },
                new Question { Text = "5. Poor appetite or overeating?", Answers = standardAnswers },
                new Question { Text = "6. Feeling bad about yourself - or that you are a failure?", Answers = standardAnswers },
                new Question { Text = "7. Trouble concentrating on things, such as reading or watching TV?", Answers = standardAnswers },
                new Question { Text = "8. Moving or speaking so slowly that other people could have noticed?", Answers = standardAnswers },
                new Question { Text = "9. Thoughts that you would be better off dead, or of hurting yourself?", Answers = standardAnswers },
                new Question { Text = "10. Feeling nervous, anxious, or on edge?", Answers = standardAnswers },
                new Question { Text = "11. Not being able to stop or control worrying?", Answers = standardAnswers },
                new Question { Text = "12. Worrying too much about different things?", Answers = standardAnswers },
                new Question { Text = "13. Trouble relaxing?", Answers = standardAnswers },
                new Question { Text = "14. Being so restless that it is hard to sit still?", Answers = standardAnswers },
                new Question { Text = "15. Becoming easily annoyed or irritable?", Answers = standardAnswers },
                new Question { Text = "16. Feeling afraid, as if something awful might happen?", Answers = standardAnswers },
                new Question { Text = "17. Feeling overwhelmed by your daily responsibilities?", Answers = standardAnswers },
                new Question { Text = "18. Avoiding social situations or friends?", Answers = standardAnswers },
                new Question { Text = "19. Difficulty managing your anger or temper?", Answers = standardAnswers },
                new Question { Text = "20. Feeling detached from reality or your surroundings?", Answers = standardAnswers }
            };

            QuestionsCollection.ItemsSource = Questions;
        }

        private void OnAnswerTapped(object sender, TappedEventArgs e)
        {
            var border = sender as Border;
            var question = border.BindingContext as Question;

            if (border != null && question != null)
            {
                int index = int.Parse(border.ClassId);
                question.SelectedScore = index;

                question.Color0 = Colors.DeepSkyBlue;
                question.Color1 = Colors.DeepSkyBlue;
                question.Color2 = Colors.DeepSkyBlue;
                question.Color3 = Colors.DeepSkyBlue;

                if (index == 0) question.Color0 = Colors.OrangeRed;
                if (index == 1) question.Color1 = Colors.OrangeRed;
                if (index == 2) question.Color2 = Colors.OrangeRed;
                if (index == 3) question.Color3 = Colors.OrangeRed;
            }
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            int totalScore = 0;
            AssessmentState.QuestionScores.Clear();

            foreach (var q in Questions)
            {
                totalScore += q.SelectedScore;
                AssessmentState.QuestionScores.Add(q.SelectedScore);
            }

            AssessmentState.CurrentScore = totalScore;

            var historyItem = new AssessmentHistoryItem
            {
                Date = DateTime.Now,
                Score = totalScore,
                Status = AssessmentState.GetStatusMessage(totalScore),
                TestType = "Wellness Assessment",
                AnswerData = new List<int>(AssessmentState.QuestionScores)
            };
            AssessmentState.History.Add(historyItem);

            await DisplayAlert("Saved", "Assessment saved to history.", "View Analysis");
            await Navigation.PushAsync(new AnalyticPage());
        }

        private async void OnNavTapped(object sender, EventArgs e)
        {
            string d = ((Button)sender).AutomationId;
            if (d == "List") return;

            if (d == "Community") await Navigation.PushAsync(new CommunityPage());
            else if (d == "Diary") await Navigation.PushAsync(new WriteDiaryPage());
            else if (d == "Stats") await Navigation.PushAsync(new AnalyticPage());
            else if (d == "Profile") await Navigation.PushAsync(new ProfilePage());
        }
    }
}