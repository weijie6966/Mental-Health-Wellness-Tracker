using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO; // Required for file copying
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            LoadProfileData();
        }

        // --- 1. Load Data on Startup ---
        private void LoadProfileData()
        {
            // Load text data
            EntryUsername.Text = Preferences.Get("UsernameKey", "New User");
            EditorBio.Text = Preferences.Get("BioKey", "Tell us about yourself.");

            // Load saved image path
            string savedImagePath = Preferences.Get("ProfileImagePath", string.Empty);

            // Check if the file actually exists before trying to display it
            if (!string.IsNullOrEmpty(savedImagePath) && File.Exists(savedImagePath))
            {
                ImgProfileAvatar.Source = ImageSource.FromFile(savedImagePath);
            }
        }

        // --- 2. Save Button Logic ---
        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            Preferences.Set("UsernameKey", EntryUsername.Text);
            Preferences.Set("BioKey", EditorBio.Text);

            await DisplayAlert("Success", "Profile updated successfully!", "OK");
        }

        // --- 3. Change Profile Logic (Fixed Persistence) ---
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
                    // 1. Create a permanent filename in the AppData directory
                    // We overwrite the same file so we don't fill up storage with old profile pics
                    string fileName = "user_profile_pic.png";
                    string permanentPath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    // 2. Copy the file there
                    using (var sourceStream = await result.OpenReadAsync())
                    using (var localFileStream = File.Create(permanentPath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }

                    // 3. Update the UI
                    ImgProfileAvatar.Source = ImageSource.FromFile(permanentPath);

                    // 4. Save this permanent path to Preferences
                    Preferences.Set("ProfileImagePath", permanentPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not pick image.", "OK");
            }
        }

        // --- 4. Other Button Actions ---
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Logout", "You have been logged out.", "OK");
            await Navigation.PopToRootAsync();
        }

        private void OnProfileImageTapped(object sender, EventArgs e) => MenuOverlay.IsVisible = true;
        private void OnOverlayTapped(object sender, EventArgs e) => MenuOverlay.IsVisible = false;

        private void OnViewProfileClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false;
            // You can add logic here to view the image full screen if you want
        }

        // --- 5. Navigation ---
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string d = ((Button)sender).AutomationId;
            if (d == "Community") await Navigation.PushAsync(new CommunityPage());
            else if (d == "List") await Navigation.PushAsync(new AssessmentPage());
            else if (d == "Diary") await Navigation.PushAsync(new WriteDiaryPage());
            else if (d == "Stats") await Navigation.PushAsync(new AnalyticPage());
        }
    }
}