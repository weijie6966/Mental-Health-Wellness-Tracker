using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic; // Required for List<byte[]>
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Views;
using Mental_Health_Wellness_Tracker.Services;
using Microsoft.Extensions.DependencyInjection; // Essential for DI navigation

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // Assuming Fody.PropertyChanged is used or you implement INPC manually
    public class WriteDiaryViewModel : ViewModelBase
    {
        // FIX 1: Inject IAssessmentRepository directly
        private readonly IAssessmentRepository _repository;

        // Data fields for the UI (using the MVVM pattern)
        public string DiaryEntryText { get; set; } // Binds to Text in XAML
        public ObservableCollection<ImageSource> SelectedImages { get; set; } = new ObservableCollection<ImageSource>();
        public string SelectedMoodEmoji { get; set; } = "emoji_neutral.png";

        // FIX 2: List to hold local paths, as per original logic (only one image in original logic)
        private string _selectedImagePath = string.Empty;
        public string SelectedImagePathDisplay { get; private set; } = "No file chosen";

        // --- Commands ---
        public ICommand MoodEmojiClickedCommand { get; }
        public ICommand UploadImagesCommand { get; }
        public ICommand PostCommand { get; }
        public ICommand NavigateCommand { get; }

        // FIX 3: Constructor accepts IAssessmentRepository via DI
        public WriteDiaryViewModel(IAssessmentRepository repository)
        {
            _repository = repository;

            MoodEmojiClickedCommand = new RelayCommand(OnMoodEmojiClicked);
            UploadImagesCommand = new RelayCommand(async _ => await OnUploadClicked()); // Renamed
            PostCommand = new RelayCommand(async _ => await OnPostClicked());
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // --- Logic (Transferred from Code-Behind) ---

        private void OnMoodEmojiClicked(object parameter)
        {
            // The XAML needs to be bound to this method
            if (parameter is string emojiFileName)
            {
                SelectedMoodEmoji = emojiFileName;
                // NOTE: Border highlighting logic should be moved to XAML using DataTriggers/Converters
            }
        }

        private async Task OnUploadClicked() // Adjusted to match original single-file upload logic
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
                    // 1. Store path for persistence model
                    _selectedImagePath = result.FullPath;

                    // 2. Update display property
                    SelectedImagePathDisplay = result.FileName;
                    OnPropertyChanged(nameof(SelectedImagePathDisplay));

                    // 3. Update preview (Optional: If you want to show the image preview in the UI)
                    // We clear and add the new image source
                    SelectedImages.Clear();
                    SelectedImages.Add(ImageSource.FromFile(result.FullPath));
                }
            }
            catch (Exception)
            {
                // Ignore cancellation or error
            }
        }

        private async Task OnPostClicked()
        {
            if (string.IsNullOrWhiteSpace(DiaryEntryText) && string.IsNullOrEmpty(_selectedImagePath))
            {
                await Application.Current.MainPage.DisplayAlert("Hold On", "Please write your diary entry before posting.", "OK");
                return;
            }

            var userId = await SecureStorage.GetAsync("user_id");
            var userEmail = await SecureStorage.GetAsync("user_email");
            var username = Preferences.Get("UsernameKey", "User");

            if (string.IsNullOrEmpty(userId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in!", "OK");
                return;
            }

            // Get mood details using the helper method
            (string moodName, int moodScore) = GetMoodDetails(SelectedMoodEmoji);

            // ✅ Map data to LocalDiaryEntry (persistence model)
            var newEntry = new LocalDiaryEntry
            {
                UserId = userId,
                UserEmail = userEmail,
                Username = username,
                Content = DiaryEntryText,

                MoodEmoji = SelectedMoodEmoji,
                MoodName = moodName,
                MoodScore = moodScore,

                ImgUrl = "", // Cloud URL is empty until background sync runs
                LocalImagePath = _selectedImagePath, // Store the local path
                DateCreated = DateTime.Now,
                IsSynced = false
            };

            // ✅ Call AddDiaryEntryAsync on the injected repository
            await _repository.AddDiaryEntryAsync(newEntry);

            await Application.Current.MainPage.DisplayAlert("Success", "Diary saved locally! Syncing in background...", "OK");

            // Clean up the UI properties
            DiaryEntryText = string.Empty;
            SelectedImagePathDisplay = "No file chosen";
            _selectedImagePath = string.Empty;
            SelectedImages.Clear();

            // 导航 to CommunityPage (using DI)
            IServiceProvider services = Application.Current?.Handler?.MauiContext?.Services;
            if (services == null) return;

            var nextPage = services.GetService<CommunityPage>();
            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
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

        // FIX 4: Navigation now uses the DI service provider
        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;

            IServiceProvider services = Application.Current?.Handler?.MauiContext?.Services;
            if (services == null) return;

            Page nextPage = destination switch
            {
                "Community" => services.GetService<CommunityPage>(),
                "List" => services.GetService<AssessmentPage>(),
                "Stats" => services.GetService<AnalyticPage>(),
                "Profile" => services.GetService<ProfilePage>(),
                "Diary" => null, // Current page
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}