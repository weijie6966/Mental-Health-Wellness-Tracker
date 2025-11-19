using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    public partial class ProfilePictureViewPage : ContentPage
    {
        public ProfilePictureViewPage(string imagePath)
        {
            InitializeComponent();

            // Set the image source based on the passed path
            if (imagePath == "nav_profile.png")
            {
                FullProfileImage.Source = "nav_profile.png";
            }
            else
            {
                FullProfileImage.Source = ImageSource.FromFile(imagePath);
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}