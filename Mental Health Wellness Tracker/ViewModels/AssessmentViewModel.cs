using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Models;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class AssessmentViewModel : ViewModelBase
    {
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        // Commands
        public ICommand SubmitCommand { get; }
        public ICommand NavigateCommand { get; }

        public AssessmentViewModel()
        {
            LoadQuestions();

            SubmitCommand = new RelayCommand(async _ => await OnSubmitClicked());
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // --- Logic (Moved from AssessmentPage.xaml.cs) ---

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
        }

        private async Task OnSubmitClicked()
        {
            // Check if all questions are answered
            if (Questions.Any(q => q.SelectedScore == -1))
            {
                await Application.Current.MainPage.DisplayAlert("Hold On", "Please answer all 20 questions before submitting.", "OK");
                return;
            }

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

            await Application.Current.MainPage.DisplayAlert("Saved", "Assessment saved to history.", "View Analysis");
            await Application.Current.MainPage.Navigation.PushAsync(new AnalyticPage());
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "List") return;

            // Mapping logic
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