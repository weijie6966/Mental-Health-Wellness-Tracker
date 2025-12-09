using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker;
using Mental_Health_Wellness_Tracker.Views;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        // Fody automatically handles INPC for these properties
        public string Username { get; set; }
        public string Bio { get; set; }
        public ImageSource ProfileAvatarSource { get; set; }
        public bool IsMenuVisible { get; set; } = false;

        // Commands
        public ICommand SaveProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ChangeProfilePictureCommand { get; }
        public ICommand ViewProfilePictureCommand { get; }
        public ICommand ToggleMenuCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand ContactUsCommand { get; } // <-- NEW COMMAND

        // Constructor and Initialization
        public ProfileViewModel()
        {
            LoadProfileData();

            // Initialize Commands
            SaveProfileCommand = new RelayCommand(async _ => await OnSaveProfileClicked());
            LogoutCommand = new RelayCommand(async _ => await OnLogoutClicked());
            ChangeProfilePictureCommand = new RelayCommand(async _ => await OnChangeProfileClicked());
            ViewProfilePictureCommand = new RelayCommand(async _ => await OnViewProfileClicked());
            ToggleMenuCommand = new RelayCommand(OnToggleMenuClicked);
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
            ContactUsCommand = new RelayCommand(async _ => await OnContactUsClicked()); // <-- INITIALIZE
        }

        // --- NEW LOGIC ---

        private async Task OnContactUsClicked()
        {
            // Navigates to the ContactUsPage located in the Views folder
            await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new ContactUsPage());
        }

        // --- Existing Logic ---

        private void LoadProfileData()
        {
            // Load text data
            Username = Preferences.Get("UsernameKey", "New User");
            Bio = Preferences.Get("BioKey", "Tell us about yourself.");

            // Load saved image path
            string savedImagePath = Preferences.Get("ProfileImagePath", string.Empty);

            // Set default or saved image
            if (!string.IsNullOrEmpty(savedImagePath) && File.Exists(savedImagePath))
            {
                ProfileAvatarSource = ImageSource.FromFile(savedImagePath);
            }
            else
            {
                ProfileAvatarSource = "nav_profile.png";
            }
        }

        private async Task OnSaveProfileClicked()
        {
            Preferences.Set("UsernameKey", Username);
            Preferences.Set("BioKey", Bio);

            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Success", "Profile updated successfully!", "OK");
        }

        private async Task OnChangeProfileClicked()
        {
            IsMenuVisible = false;

            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a profile picture",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    string fileName = "user_profile_pic.png";
                    string permanentPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    // 2. Copy the file there
                    using (var sourceStream = await result.OpenReadAsync())
                    using (var localFileStream = File.Create(permanentPath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }

                    // 3. Update the UI bound property
                    ProfileAvatarSource = ImageSource.FromFile(permanentPath);

                    // 4. Save this permanent path to Preferences
                    Preferences.Set("ProfileImagePath", permanentPath);
                }
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", $"Could not pick image: {ex.Message}", "OK");
            }
        }

        private void OnToggleMenuClicked(object parameter)
        {
            IsMenuVisible = !IsMenuVisible;
        }

        private async Task OnViewProfileClicked()
        {
            IsMenuVisible = false;

            string currentImagePath = Preferences.Get("ProfileImagePath", "nav_profile.png");

            // Navigate to the ProfilePictureViewPage, passing the path.
            await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new ProfilePictureViewPage(currentImagePath));
        }

        private async Task OnLogoutClicked()
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Logout", "You have been logged out.", "OK");
            // Use PopToRootAsync to return to the root (Login) screen
            await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;

            // Map string command parameter to Page instance (using simple push/pop model)
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
                await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}