using Microsoft.Maui.Controls;
using System;

namespace Mental_Health_Wellness_Tracker
{
    public partial class ProfilePictureViewPage : ContentPage
    {
        public ProfilePictureViewPage(string imagePath)
        {
            InitializeComponent();

            // Set the image source based on the passed path
            if (string.IsNullOrWhiteSpace(imagePath) || imagePath == "nav_profile.png")
            {
                FullProfileImage.Source = "nav_profile.png";
            }
            else if (Uri.TryCreate(imagePath, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                FullProfileImage.Source = ImageSource.FromUri(uri);
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