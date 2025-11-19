using Microsoft.Maui.Controls;
using System;

namespace Mental_Health_Wellness_Tracker
{
    public partial class MainPage : ContentPage
    {
        // Variable to track if the password is visible or hidden
        private bool _isPasswordVisible = false;

        public MainPage()
        {
            InitializeComponent();
        }

        // 1. Login Button Logic (Navigates to Diary Page)
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // --- Basic Logic ---
            // In a real application, you would validate the username/password against a database here.
            // For now, we assume successful login and navigate to the main app screen.

            // Navigate to the "Write Diary" Page
            await Navigation.PushAsync(new WriteDiaryPage());
        }

        // 2. Show/Hide Password Logic (Eye Icon)
        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            // Toggle the state
            _isPasswordVisible = !_isPasswordVisible;

            // Update the Entry property (EntryPassword is named in the XAML)
            EntryPassword.IsPassword = !_isPasswordVisible;

            // Swap the image source based on the state
            if (_isPasswordVisible)
            {
                BtnTogglePassword.Source = "eye_closed.png";
            }
            else
            {
                BtnTogglePassword.Source = "eye_open.png";
            }
        }

        // 3. Create New Account Logic (Footer Link)
        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            // Navigate to the Sign Up Page
            await Navigation.PushAsync(new SignUpPage());
        }

        // 4. Forgot Password Logic (Footer Link)
        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            // Navigate to the Forgot Password Recovery Page
            await Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}