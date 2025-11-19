using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker
{
    // Model for a single question
    public class Question
    {
        public string Text { get; set; } = string.Empty;
        public List<string> Answers { get; set; } = new List<string>();
        // You could add properties for SelectedAnswer, Score, etc.
    }

    public partial class AssessmentPage : ContentPage
    {
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        public AssessmentPage()
        {
            InitializeComponent();
            LoadQuestions();

            // Required to set the context for the CollectionView to find the 'Questions' property
            this.BindingContext = this;
        }

        private void LoadQuestions()
        {
            Questions = new ObservableCollection<Question>
            {
                new Question
                {
                    Text = "Question 1: How often do you feel down, depressed, or hopeless?",
                    Answers = new List<string> { "Not at all", "Several days", "More than half the days", "Nearly every day" }
                },
                new Question
                {
                    Text = "Question 2: How much interest or pleasure do you have in doing things?",
                    Answers = new List<string> { "Not at all", "Several days", "More than half the days", "Nearly every day" }
                },
                new Question
                {
                    Text = "Question 3: How often are you bothered by trouble falling or staying asleep?",
                    Answers = new List<string> { "Not at all", "Several days", "More than half the days", "Nearly every day" }
                }
            };

            QuestionsCollection.ItemsSource = Questions;
        }

        // Navigation logic for the bottom bar
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage());
            }
            else if (destination == "Diary")
            {
                await Navigation.PushAsync(new WriteDiaryPage());
            }
            else if (destination == "Stats")
            {
                await Navigation.PushAsync(new AnalyticPage());
            }
            else if (destination == "Profile")
            {
                await Navigation.PushAsync(new ProfilePage());
            }
        }
    }
}