using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // Needed for FilePicker & Preferences
using System;
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

        // --- 1. Profile Data Loading ---
        private void LoadProfileData()
        {
            // Load text data (Username/Bio) from Preferences
            EntryUsername.Text = Preferences.Get("UsernameKey", "New User");
            EditorBio.Text = Preferences.Get("BioKey", "Tell us about yourself.");

            // Load saved image path (if any) and set it to the Image control
            string savedImagePath = Preferences.Get("ProfileImagePath", string.Empty);
            if (!string.IsNullOrEmpty(savedImagePath))
            {
                ImgProfileAvatar.Source = ImageSource.FromFile(savedImagePath);
            }
        }

        // --- 2. Main Button Actions ---
        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
            // Save the text fields to persistent storage
            Preferences.Set("UsernameKey", EntryUsername.Text);
            Preferences.Set("BioKey", EditorBio.Text);

            await DisplayAlert("Success", "Profile updated successfully!", "OK");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // In a real app, clear session tokens here
            await DisplayAlert("Logout", "You have been logged out.", "OK");

            // Return to the login screen
            await Navigation.PopToRootAsync();
        }

        // --- 3. POPUP MENU LOGIC (Grid Overlay) ---

        // Open the menu when the profile picture is tapped
        private void OnProfileImageTapped(object sender, EventArgs e)
        {
            // Show the overlay grid defined in XAML
            MenuOverlay.IsVisible = true;
        }

        // Close the menu when tapping the semi-transparent background
        private void OnOverlayTapped(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false;
        }

        // Action: View Profile (UPDATED: Shows ONLY the profile picture)
        private async void OnViewProfileClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false; // Close menu first

            // Check if the user has a custom image or is using the default
            string imageSource = "nav_profile.png"; // Default
            string savedPath = Preferences.Get("ProfileImagePath", string.Empty);

            if (!string.IsNullOrEmpty(savedPath))
            {
                // If using a local file path, we need to display it differently or just confirm it exists
                // For simplicity in this context, we will show a focused modal or alert.
                // Since standard alerts can't show images easily, we will navigate to a temporary
                // "Image View" page or just confirm the action.

                // OPTION A: Simple Alert (Text only - "Viewing Picture")
                // await DisplayAlert("Profile Picture", "Displaying full-size image...", "Close");

                // OPTION B: (Recommended) Navigate to a dedicated page to view the image
                await Navigation.PushAsync(new ProfilePictureViewPage(savedPath));
            }
            else
            {
                // Default image logic
                await Navigation.PushAsync(new ProfilePictureViewPage("nav_profile.png"));
            }
        }

        // Action: Change Profile (File Picker Logic)
        private async void OnChangeProfileClicked(object sender, EventArgs e)
        {
            MenuOverlay.IsVisible = false; // Close menu first

            try
            {
                // Open the device's photo picker
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a profile picture",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Update the UI immediately with the new image
                    ImgProfileAvatar.Source = ImageSource.FromFile(result.FullPath);

                    // Save the path to Preferences so it loads next time
                    Preferences.Set("ProfileImagePath", result.FullPath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not pick image. Permissions might be denied.", "OK");
            }
        }

        // --- 4. Bottom Navigation Logic ---
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            // Standard navigation routing
            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage());
            }
            else if (destination == "List")
            {
                await Navigation.PushAsync(new AssessmentPage());
            }
            else if (destination == "Diary")
            {
                await Navigation.PushAsync(new WriteDiaryPage());
            }
            else if (destination == "Stats")
            {
                await Navigation.PushAsync(new AnalyticPage());
            }
            // No need for "Profile" logic since we are already on this page.
        }
    }
}