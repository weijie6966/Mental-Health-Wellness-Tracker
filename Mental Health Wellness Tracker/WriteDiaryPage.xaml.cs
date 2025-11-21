using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;
using Mental_Health_Wellness_Tracker.Services; // Import Services
using Mental_Health_Wellness_Tracker.Models;   // Import Models

namespace Mental_Health_Wellness_Tracker
{
    public partial class WriteDiaryPage : ContentPage
    {
        private string _selectedMoodEmoji = "emoji_neutral.png";    // Default mood
        private readonly IAssessmentRepository _repository;         // Data repository

        public WriteDiaryPage()
        {
            InitializeComponent();

            // Manually obtain the repository from the system service
            _repository = IPlatformApplication.Current.Services.GetService<IAssessmentRepository>();

            HighlightSelectedMoodEmoji();
        }

        // --- Mood Selection Logic ---
        private void OnMoodEmojiClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is string emojiFileName)
            {
                _selectedMoodEmoji = emojiFileName;
                HighlightSelectedMoodEmoji();
            }
        }

        private void HighlightSelectedMoodEmoji()
        {
            EmojiDead.BorderWidth = 0;
            EmojiSad.BorderWidth = 0;
            EmojiNeutral.BorderWidth = 0;
            EmojiSmile.BorderWidth = 0;
            EmojiLove.BorderWidth = 0;

            if (_selectedMoodEmoji == "emoji_dead.png") EmojiDead.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_sad.png") EmojiSad.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_neutral.png") EmojiNeutral.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_smile.png") EmojiSmile.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_love.png") EmojiLove.BorderWidth = 3;
        }

        private async void OnUploadClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a photo",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null) LblFileName.Text = result.FileName;
            }
            catch (Exception ex) { }
        }

        // Logic for "Post to Community" button
        private async void OnPostClicked(object sender, EventArgs e)
        {
            string diaryText = TxtDiaryEntry.Text;

            if (string.IsNullOrWhiteSpace(diaryText))
            {
                await DisplayAlert("Hold On", "Please write your diary entry before posting.", "OK");
                return;
            }

            // Get current user ID
            string userId = await SecureStorage.GetAsync("user_id") ?? "unknown_user";

            var newEntry = new DiaryEntry
            {
                UserId = userId,
                Username = Preferences.Get("UsernameKey", "New User"),
                Content = diaryText,
                MoodEmoji = _selectedMoodEmoji,
                DateCreated = DateTime.Now,
                IsSynced = false
            };

            // Save to database
            bool isSaved = await _repository.SaveDiaryEntryAsync(newEntry);

            if (isSaved)
            {
                await DisplayAlert("Success", "Diary saved to your private journal!", "OK");

                // Navigate to the Community Page
                // CommunityPage will automatically load the latest diary entries from the database
                await Navigation.PushAsync(new CommunityPage(_repository));
            }
            else
            {
                await DisplayAlert("Error", "Failed to save diary.", "OK");
            }
        }

        // Bottom Navigation Logic
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage(_repository));
            }
            else if (destination == "List")
            {
                var repo = IPlatformApplication.Current.Services.GetService<Services.IAssessmentRepository>();
                await Navigation.PushAsync(new AssessmentPage(repo));
            }
            else if (destination == "Stats")
            {
                var repo = IPlatformApplication.Current.Services.GetService<Services.IAssessmentRepository>();
                await Navigation.PushAsync(new AnalyticPage(repo));
            }
            else if (destination == "Profile")
            {
                await Navigation.PushAsync(new ProfilePage());
            }
        }
    }
}