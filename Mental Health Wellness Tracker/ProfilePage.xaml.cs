using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models; // Need Models for UserProfile

namespace Mental_Health_Wellness_Tracker
{
    public partial class ProfilePage : ContentPage
    {
        // Repository injection
        private readonly IAssessmentRepository _repository;
        private string _currentUserId;

        // Default constructor (used by App Shell or Navigation sometimes)
        public ProfilePage()
        {
            InitializeComponent();
            // Service Locator as fallback
            _repository = IPlatformApplication.Current.Services.GetService<IAssessmentRepository>();
            LoadProfileData();
        }

        // Recommended constructor with injection
        public ProfilePage(IAssessmentRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            LoadProfileData();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Reload data every time the page appears to ensure freshness
            await LoadProfileData();
        }

        // --- 1. Profile Data Loading (From Database) ---
        private async Task LoadProfileData()
        {
            // Get current UserID
            _currentUserId = await SecureStorage.GetAsync("user_id");

            if (string.IsNullOrEmpty(_currentUserId))
            {
                // If not logged in (shouldn't happen here), show defaults
                EntryUsername.Text = "Guest";
                EditorBio.Text = "Please log in.";
                return;
            }

            // Fetch Profile from Database
            var profile = await _repository.GetUserProfileAsync(_currentUserId);

            if (profile != null)
            {
                EntryUsername.Text = profile.Username;
                EditorBio.Text = profile.Bio;

                // While loading data, update the local cache to ensure that the name can be retrieved when writing a diary entry.
                Preferences.Set("UsernameKey", profile.Username);

                // Load image if path exists
                if (!string.IsNullOrEmpty(profile.ProfileImagePath))
                {
                    ImgProfileAvatar.Source = ImageSource.FromFile(profile.ProfileImagePath);
                }
            }
            else
            {
                // No profile yet? Set defaults (but don't save yet)
                EntryUsername.Text = "";
                EditorBio.Text = "";
            }
        }

        // --- 2. Main Button Actions ---
        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUserId)) return;

            string newUsername = EntryUsername.Text;
            string newBio = EditorBio.Text;

            // 1. Create or Update Profile Model
            var profile = new UserProfile
            {
                UserId = _currentUserId,
                Username = newUsername,
                Bio = newBio,
                // Keep existing image path if we haven't changed it here (logic simplified)
                // For now, we rely on the file picker saving to Preferences or we need to track it.
                // Let's grab the image path from Preferences as a temporary holding spot or track it in a field.
                ProfileImagePath = Preferences.Get("TempProfileImagePath", "")
            };

            // 2. Save to Database
            bool success = await _repository.SaveUserProfileAsync(profile);

            // 3. Also update Preferences for the "UsernameKey" so WriteDiaryPage can find it easily
            // (This keeps compatibility with your other pages without refactoring everything)
            Preferences.Set("UsernameKey", newUsername);

            if (success)
                await DisplayAlert("Success", "Profile updated successfully!", "OK");
            else
                await DisplayAlert("Error", "Failed to update profile.", "OK");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Clear session
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");

            // Optional: Clear username pref so next user doesn't see it briefly
            Preferences.Remove("UsernameKey");

            await DisplayAlert("Logout", "You have been logged out.", "OK");
            await Navigation.PopToRootAsync();
        }

        // --- 3. POPUP MENU LOGIC (Grid Overlay) ---

        private void OnProfileImageTapped(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = true;
        }

        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false;
        }

        private async void OnViewProfileClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false;
            // Logic to view image (omitted for brevity, similar to before)
            string savedPath = Preferences.Get("TempProfileImagePath", string.Empty);
            if (!string.IsNullOrEmpty(savedPath))
                await Navigation.PushAsync(new ProfilePictureViewPage(savedPath));
            else
                await Navigation.PushAsync(new ProfilePictureViewPage("nav_profile.png"));
        }

        private async void OnChangeProfileClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false;

            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a profile picture",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    ImgProfileAvatar.Source = ImageSource.FromFile(result.FullPath);
                    // Save temporarily to Prefs so SaveProfileClicked can grab it
                    Preferences.Set("TempProfileImagePath", result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not pick image.", "OK");
            }
        }

        // --- 4. Bottom Navigation Logic ---
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community") await Navigation.PushAsync(new CommunityPage(_repository));
            else if (destination == "List") await Navigation.PushAsync(new AssessmentPage(_repository));
            else if (destination == "Diary") await Navigation.PushAsync(new WriteDiaryPage());
            else if (destination == "Stats") await Navigation.PushAsync(new AnalyticPage(_repository));
        }
    }
}