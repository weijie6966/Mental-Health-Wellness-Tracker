using Microsoft.Maui.Controls;
using System;

namespace Mental_Health_Wellness_Tracker
{
    public partial class SignUpPage : ContentPage
    {
        // Variables to track the visibility state of the two password fields
        private bool _isPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public SignUpPage()
        {
            InitializeComponent();
        }

        // 1. Logic for the Main Password Eye Icon
        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            // Toggle the state
            _isPasswordVisible = !_isPasswordVisible;

            // Show/Hide the text
            EntryPassword.IsPassword = !_isPasswordVisible;

            // Swap the image source
            if (_isPasswordVisible)
                BtnTogglePassword.Source = "eye_closed.png"; // User can see text
            else
                BtnTogglePassword.Source = "eye_open.png";   // User sees dots
        }

        // 2. Logic for the Confirm Password Eye Icon
        private void OnToggleConfirmPasswordClicked(object sender, EventArgs e)
        {
            // Toggle the state
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;

            // Show/Hide the text
            EntryConfirmPassword.IsPassword = !_isConfirmPasswordVisible;

            // Swap the image source
            if (_isConfirmPasswordVisible)
                BtnToggleConfirmPassword.Source = "eye_closed.png";
            else
                BtnToggleConfirmPassword.Source = "eye_open.png";
        }

        // 3. Sign Up Button Logic
        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            // A. Basic Validation: Check if fields are empty
            if (string.IsNullOrWhiteSpace(EntryPassword.Text) || string.IsNullOrWhiteSpace(EntryConfirmPassword.Text))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // B. Validation: Check if passwords match
            if (EntryPassword.Text != EntryConfirmPassword.Text)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            // C. Success: Navigate to the "Success/Congratulations" Page
            // This moves to the page you created in the previous step
            await Navigation.PushAsync(new SignUpSuccessPage());
        }

        // 4. Back Button Logic (Top Left Arrow & Footer Link)
        private async void OnBackArrowClicked(object sender, EventArgs e)
        {
            // Returns to the Login Page
            await Navigation.PopAsync();
        }
    }
}