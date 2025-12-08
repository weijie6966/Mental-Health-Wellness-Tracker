using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Models;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class WriteDiaryViewModel : ViewModelBase
    {
        private readonly List<byte[]> _collectedImageBytes = new List<byte[]>();

        // Data Properties (Fody handles INPC for these)
        public string DiaryEntryText { get; set; }
        public ObservableCollection<ImageSource> SelectedImages { get; set; } = new ObservableCollection<ImageSource>();
        public string SelectedMoodEmoji { get; set; } = "emoji_neutral.png";

        // --- Commands ---
        public ICommand MoodEmojiClickedCommand { get; }
        public ICommand UploadImagesCommand { get; }
        public ICommand PostCommand { get; }
        public ICommand NavigateCommand { get; }

        public WriteDiaryViewModel()
        {
            MoodEmojiClickedCommand = new RelayCommand(OnMoodEmojiClicked);
            UploadImagesCommand = new RelayCommand(async _ => await OnUploadImagesClicked());
            PostCommand = new RelayCommand(async _ => await OnPostClicked());
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // --- Logic (Moved from WriteDiaryPage.xaml.cs) ---

        private void OnMoodEmojiClicked(object parameter)
        {
            if (parameter is string emojiFileName)
            {
                SelectedMoodEmoji = emojiFileName;
            }
        }

        private async Task OnUploadImagesClicked()
        {
            try
            {
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
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task OnPostClicked()
        {
            if (string.IsNullOrWhiteSpace(DiaryEntryText))
            {
                await Application.Current.MainPage.DisplayAlert("Hold On", "Please write your diary entry before posting.", "OK");
                return;
            }

            string currentUsername = Preferences.Get("UsernameKey", "New User");
            string currentUserImage = Preferences.Get("ProfileImagePath", "user_icon_placeholder.png");

            var newPost = new Post
            {
                Username = currentUsername,
                UserProfileImage = currentUserImage,
                Content = DiaryEntryText,
                MoodEmoji = SelectedMoodEmoji,
                Likes = 0,
                Comments = 0,
                PostTime = DateTime.Now
            };

            // Add all collected images to the Post
            foreach (var imgBytes in _collectedImageBytes)
            {
                // Must clone the memory stream here as the Post object is passed to a new page
                newPost.PostImages.Add(ImageSource.FromStream(() => new MemoryStream(imgBytes)));
            }

            // Navigate to CommunityPage, passing the new post
            await Application.Current.MainPage.Navigation.PushAsync(new CommunityPage(newPost));
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Diary") return;

            // Mapping logic
            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "List" => new AssessmentPage(),
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