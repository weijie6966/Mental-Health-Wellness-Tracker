using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker
{
    public partial class WriteDiaryPage : ContentPage
    {
        private string _selectedMoodEmoji = "emoji_neutral.png";

        // Store selected images as sources for preview
        public ObservableCollection<ImageSource> SelectedImages { get; set; } = new ObservableCollection<ImageSource>();

        // Store raw data to pass to the Post
        private List<byte[]> _collectedImageBytes = new List<byte[]>();

        public WriteDiaryPage()
        {
            InitializeComponent();
            HighlightSelectedMoodEmoji();
            SelectedImagesCollection.ItemsSource = SelectedImages;
        }

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
            EmojiDead.BorderWidth = 0; EmojiSad.BorderWidth = 0; EmojiNeutral.BorderWidth = 0; EmojiSmile.BorderWidth = 0; EmojiLove.BorderWidth = 0;
            if (_selectedMoodEmoji == "emoji_dead.png") EmojiDead.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_sad.png") EmojiSad.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_neutral.png") EmojiNeutral.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_smile.png") EmojiSmile.BorderWidth = 3;
            else if (_selectedMoodEmoji == "emoji_love.png") EmojiLove.BorderWidth = 3;
        }

        // --- UPDATED UPLOAD LOGIC: Multiple Files ---
        private async void OnUploadClicked(object sender, EventArgs e)
        {
            try
            {
                // ALLOW MULTIPLE SELECTION
                var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Select photos",
                    FileTypes = FilePickerFileType.Images
                });

                if (results != null && results.Any())
                {
                    foreach (var file in results)
                    {
                        // 1. Convert to Byte Array (for data storage)
                        using (var stream = await file.OpenReadAsync())
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            byte[] bytes = memoryStream.ToArray();
                            _collectedImageBytes.Add(bytes);

                            // 2. Add to Preview List (for UI)
                            SelectedImages.Add(ImageSource.FromStream(() => new MemoryStream(bytes)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnPostClicked(object sender, EventArgs e)
        {
            string diaryText = TxtDiaryEntry.Text;
            if (string.IsNullOrWhiteSpace(diaryText)) { await DisplayAlert("Hold On", "Please write your diary entry before posting.", "OK"); return; }

            string currentUsername = Preferences.Get("UsernameKey", "New User");
            string currentUserImage = Preferences.Get("ProfileImagePath", "user_icon_placeholder.png");

            var newPost = new Post
            {
                Username = currentUsername,
                UserProfileImage = currentUserImage,
                Content = diaryText,
                MoodEmoji = _selectedMoodEmoji,
                Likes = 0,
                Comments = 0,
                PostTime = DateTime.Now
            };

            // Add all collected images to the Post
            foreach (var imgBytes in _collectedImageBytes)
            {
                newPost.PostImages.Add(ImageSource.FromStream(() => new MemoryStream(imgBytes)));
            }

            await Navigation.PushAsync(new CommunityPage(newPost));
        }

        private async void OnNavTapped(object sender, EventArgs e)
        {
            string d = ((Button)sender).AutomationId;
            if (d == "Community") await Navigation.PushAsync(new CommunityPage());
            else if (d == "List") await Navigation.PushAsync(new AssessmentPage());
            else if (d == "Stats") await Navigation.PushAsync(new AnalyticPage());
            else if (d == "Profile") await Navigation.PushAsync(new ProfilePage());
        }
    }
}