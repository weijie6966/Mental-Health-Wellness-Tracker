using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Views;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // Assuming Fody.PropertyChanged or manual INPC for UI updates
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository = new AssessmentRepository();
        private UserProfile _userProfile; // Model to hold profile data

        // FIX: Properties now rely on the _userProfile model
        public string Username
        {
            get => _userProfile.Username;
            set { _userProfile.Username = value; OnPropertyChanged(); }
        }
        public string Bio
        {
            get => _userProfile.Bio;
            set { _userProfile.Bio = value; OnPropertyChanged(); }
        }

        public ImageSource ProfileAvatarSource { get; set; } = "nav_profile.png"; // Default image
        public bool IsMenuVisible { get; set; } = false;

        // Commands (initialization remains the same)
        public ICommand SaveProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ChangeProfilePictureCommand { get; }
        public ICommand ViewProfilePictureCommand { get; }
        public ICommand ToggleMenuCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand ContactUsCommand { get; }
        public ICommand AppearingCommand { get; } // For loading data OnAppearing

        public ProfileViewModel()
        {
            _userProfile = new UserProfile();

            // Initialize Commands
            SaveProfileCommand = new RelayCommand(async _ => await OnSaveProfileClicked());
            LogoutCommand = new RelayCommand(async _ => await OnLogoutClicked());
            ChangeProfilePictureCommand = new RelayCommand(async _ => await OnChangeProfileClicked());
            ViewProfilePictureCommand = new RelayCommand(async _ => await OnViewProfileClicked());
            ToggleMenuCommand = new RelayCommand(OnToggleMenuClicked);
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
            ContactUsCommand = new RelayCommand(async _ => await OnContactUsClicked());
            AppearingCommand = new RelayCommand(async _ => await LoadProfileData());

            // Run initial load (OnAppearing will trigger the full reload later)
            Task.Run(LoadProfileData);
        }

        // --- Data Loading & Saving (Business Logic) ---

        public async Task LoadProfileData() // FIX 2: Load from repository
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                string userId = await SecureStorage.GetAsync("user_id");
                if (string.IsNullOrEmpty(userId)) return;

                // Fetch data from repository
                var profile = await _repository.GetUserProfileAsync(userId);

                if (profile != null)
                {
                    _userProfile = profile;

                    // Update local preference cache (for compatibility with WriteDiaryViewModel)
                    Preferences.Set("UsernameKey", profile.Username);

                    // Trigger UI update for bound properties
                    OnPropertyChanged(nameof(Username));
                    OnPropertyChanged(nameof(Bio));

                    // Load and set avatar source
                    if (!string.IsNullOrEmpty(_userProfile.ProfileImagePath))
                    {
                        if (Uri.TryCreate(_userProfile.ProfileImagePath, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                        {
                            ProfileAvatarSource = ImageSource.FromUri(uri);
                        }
                        else if (File.Exists(_userProfile.ProfileImagePath))
                        {
                            ProfileAvatarSource = ImageSource.FromFile(_userProfile.ProfileImagePath);
                        }
                        else
                        {
                            ProfileAvatarSource = "nav_profile.png";
                        }
                    }
                    else
                    {
                        ProfileAvatarSource = "nav_profile.png";
                    }
                    OnPropertyChanged(nameof(ProfileAvatarSource));
                }
                else
                {
                    _userProfile = new UserProfile { UserId = userId };
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OnSaveProfileClicked() // FIX 3: Save to repository
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                // The properties (Username, Bio) are already updated via the setters,
                // so we just need to update the remaining model fields and persist.
                var userId = await SecureStorage.GetAsync("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "You need to sign in before saving a profile.", "OK");
                    return;
                }

                _userProfile.UserId = userId;
                _userProfile.LastUpdated = DateTime.Now;

                bool success = await _repository.SaveUserProfileAsync(_userProfile);
                if (success)
                {
                    // Update local preference cache (for compatibility with WriteDiaryViewModel)
                    Preferences.Set("UsernameKey", _userProfile.Username);
                    await Application.Current.MainPage.DisplayAlert("Success", "Profile updated successfully!", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to update profile.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        // --- Image/File Logic ---

        private async Task OnChangeProfileClicked()
        {
            IsMenuVisible = false;

            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Select a profile picture", FileTypes = FilePickerFileType.Images });

                if (result != null)
                {
                    var userId = await SecureStorage.GetAsync("user_id");
                    var token = await SecureStorage.GetAsync("auth_token");

                    if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                    {
                        await Application.Current.MainPage.DisplayAlert("Login required", "Please sign in before updating your profile picture.", "OK");
                        return;
                    }

                    if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                    {
                        await Application.Current.MainPage.DisplayAlert("Offline", "Connect to the internet to upload your profile picture.", "OK");
                        return;
                    }

                    using var sourceStream = await result.OpenReadAsync();
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(result.FileName)}";
                    var storageService = new FirebaseStorageService();
                    var downloadUrl = await storageService.UploadImageAsync(sourceStream, fileName, token, "profile_images");

                    if (string.IsNullOrEmpty(downloadUrl))
                    {
                        await Application.Current.MainPage.DisplayAlert("Upload failed", "Could not upload the image. Please try again.", "OK");
                        return;
                    }

                    _userProfile.UserId = userId;
                    _userProfile.ProfileImagePath = downloadUrl;
                    ProfileAvatarSource = ImageSource.FromUri(new Uri(downloadUrl));
                    OnPropertyChanged(nameof(ProfileAvatarSource));

                    await _repository.SaveUserProfileAsync(_userProfile);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not pick image: {ex.Message}", "OK");
            }
        }

        private async Task OnViewProfileClicked()
        {
            IsMenuVisible = false;

            await Application.Current.MainPage.Navigation.PushAsync(new ProfilePictureViewPage(_userProfile.ProfileImagePath));
        }

        // --- Command Logic ---

        private void OnToggleMenuClicked(object parameter)
        {
            IsMenuVisible = !IsMenuVisible;
        }

        private async Task OnLogoutClicked()
        {
            // Clear session data
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");
            Preferences.Clear();

            await Application.Current.MainPage.DisplayAlert("Logout", "You have been logged out.", "OK");

            // Navigate to the root (Login) screen
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        // FIX 5: Navigation method for Contact Us page (uses DI)
        private async Task OnContactUsClicked()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ContactUsPage());
        }

        // FIX 6: All bottom navigation uses DI
        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Profile") return;

            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "List" => new AssessmentPage(),
                "Diary" => new WriteDiaryPage(),
                "Stats" => new AnalyticPage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}