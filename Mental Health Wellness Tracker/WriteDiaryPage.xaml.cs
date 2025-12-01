using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;

namespace Mental_Health_Wellness_Tracker
{
    public partial class WriteDiaryPage : ContentPage
    {
        private string _selectedMoodEmoji = "emoji_neutral.png";
        private readonly IAssessmentRepository _repository;
        // Used to temporarily store the image path selected by the user
        private string _selectedImagePath = string.Empty;

        public WriteDiaryPage()
        {
            InitializeComponent();
            _repository = IPlatformApplication.Current.Services.GetService<IAssessmentRepository>();
            HighlightSelectedMoodEmoji();
        }

        // --- 心情选择逻辑 ---
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

                if (result != null)
                {
                    LblFileName.Text = result.FileName;
                    // Save local path to variable
                    _selectedImagePath = result.FullPath;
                }
            }
            catch (Exception ex) 
            {
                // Ignore cancellation or error
            }
        }

        // --- 核心发布逻辑 ---
        private async void OnPostClicked(object sender, EventArgs e)
        {
            string diaryText = TxtDiaryEntry.Text;

            if (string.IsNullOrWhiteSpace(diaryText) && string.IsNullOrEmpty(_selectedImagePath))
            {
                await DisplayAlert("Hold On", "Please write your diary entry before posting.", "OK");
                return;
            }

            var userId = await SecureStorage.GetAsync("user_id");
            var userEmail = await SecureStorage.GetAsync("user_email");
            var username = Preferences.Get("UsernameKey", "User");

            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "You are not logged in!", "OK");
                return;
            }

            // 获取心情详情
            (string moodName, int moodScore) = GetMoodDetails(_selectedMoodEmoji);

            // ✅ 创建 LocalDiaryEntry (本地模型)
            var newEntry = new LocalDiaryEntry
            {
                UserId = userId,
                UserEmail = userEmail,
                Username = username,
                Content = diaryText,

                MoodEmoji = _selectedMoodEmoji,
                MoodName = moodName,
                MoodScore = moodScore,

                ImgUrl = "", // The initial cloud connection is empty
                LocalImagePath = _selectedImagePath,
                DateCreated = DateTime.Now,
                IsSynced = false
            };

            // ✅ 调用 AddDiaryEntryAsync
            await _repository.AddDiaryEntryAsync(newEntry);

            await DisplayAlert("Success", "Diary saved locally! Syncing in background...", "OK");

            // Clean up the UI
            TxtDiaryEntry.Text = string.Empty;
            LblFileName.Text = "No file chosen";
            _selectedImagePath = string.Empty; // Reset Path

            // 导航
            await Navigation.PushAsync(new CommunityPage(_repository));
        }

        private (string name, int score) GetMoodDetails(string emojiFile)
        {
            switch (emojiFile)
            {
                case "emoji_dead.png": return ("Super unhappy", 1);
                case "emoji_sad.png": return ("unhappy", 2);
                case "emoji_neutral.png": return ("normal", 3);
                case "emoji_smile.png": return ("happy", 4);
                case "emoji_love.png": return ("super happy", 5);
                default: return ("normal", 3);
            }
        }

        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;
            if (destination == "Community") await Navigation.PushAsync(new CommunityPage(_repository));
            else if (destination == "List") await Navigation.PushAsync(new AssessmentPage(_repository));
            else if (destination == "Stats") await Navigation.PushAsync(new AnalyticPage(_repository));
            else if (destination == "Profile") await Navigation.PushAsync(new ProfilePage());
        }
    }
}