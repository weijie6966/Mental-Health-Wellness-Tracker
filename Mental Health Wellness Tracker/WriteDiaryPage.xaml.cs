using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker
{
    public partial class WriteDiaryPage : ContentPage
    {
        private string _selectedMoodEmoji = "emoji_neutral.png"; // Default mood

        public WriteDiaryPage()
        {
            InitializeComponent();
            HighlightSelectedMoodEmoji(); // Ensure a default highlight on load
        }

        // --- New Mood Selection Logic ---
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
            // Reset all borders
            EmojiDead.BorderWidth = 0;
            EmojiSad.BorderWidth = 0;
            EmojiNeutral.BorderWidth = 0;
            EmojiSmile.BorderWidth = 0;
            EmojiLove.BorderWidth = 0;

            // Apply highlight to the selected one
            if (_selectedMoodEmoji == "emoji_dead.png") EmojiDead.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_sad.png") EmojiSad.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_neutral.png") EmojiNeutral.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_smile.png") EmojiSmile.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_love.png") EmojiLove.BorderWidth = 3;
        }


        // 1. Logic for "Choose File" button
        private async void OnUploadClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a photo for your diary entry",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    LblFileName.Text = result.FileName;
                }
                else
                {
                    LblFileName.Text = "No file chosen";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Could not pick file: {ex.Message}", "OK");
            }
        }

        // 2. Logic for "Post to Community" button
        private async void OnPostClicked(object sender, EventArgs e)
        {
            string diaryText = TxtDiaryEntry.Text;

            if (string.IsNullOrWhiteSpace(diaryText))
            {
                await DisplayAlert("Hold On", "Please write your diary entry before posting.", "OK");
                return;
            }

            // Create the new Post object (Post class is defined in CommunityPage.xaml.cs)
            var newPost = new Post
            {
                Username = "New_User",
                Content = diaryText,
                MoodEmoji = _selectedMoodEmoji, // Pass the selected mood emoji
                Likes = 0, // New posts start with 0 likes
                Comments = 0 // New posts start with 0 comments
            };

            // Navigate to the Community Page, passing the post data
            await Navigation.PushAsync(new CommunityPage(newPost));
        }

        // 3. Bottom Navigation Logic
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage());
            }
            else if (destination == "List")
            {
                await Navigation.PushAsync(new AssessmentPage());
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