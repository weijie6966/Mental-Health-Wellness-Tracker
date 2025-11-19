using Microsoft.Maui.Controls;
using System;

namespace Mental_Health_Wellness_Tracker
{
    public partial class ForgotPasswordPage : ContentPage
    {
        // State trackers for the two password fields
        private bool _isNewPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        // Logic for New Password Eye Icon
        private void OnToggleNewPasswordClicked(object sender, EventArgs e)
        {
            _isNewPasswordVisible = !_isNewPasswordVisible;
            EntryNewPassword.IsPassword = !_isNewPasswordVisible;

            if (_isNewPasswordVisible)
                BtnToggleNewPassword.Source = "eye_closed.png";
            else
                BtnToggleNewPassword.Source = "eye_open.png";
        }

        // Logic for Confirm Password Eye Icon
        private void OnToggleConfirmPasswordClicked(object sender, EventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
            EntryConfirmPassword.IsPassword = !_isConfirmPasswordVisible;

            if (_isConfirmPasswordVisible)
                BtnToggleConfirmPassword.Source = "eye_closed.png";
            else
                BtnToggleConfirmPassword.Source = "eye_open.png";
        }

        // Logic for the SEND Verification Code button
        private async void OnSendVerificationClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryEmail.Text))
            {
                await DisplayAlert("Error", "Please enter your email address first.", "OK");
                return;
            }

            // In a real app, this is where you would call an API service 
            // to send a verification code to EntryEmail.Text
            await DisplayAlert("Sent", $"Verification code sent to {EntryEmail.Text}", "OK");
        }

        // Logic for the final password reset (You would call this on a Reset button)
        private async void OnResetPasswordClicked(object sender, EventArgs e)
        {
            // Add robust validation here (code match, password match, etc.)
            if (EntryNewPassword.Text != EntryConfirmPassword.Text)
            {
                await DisplayAlert("Error", "New passwords do not match.", "OK");
                return;
            }

            // If validation passes, call API to reset password
            await DisplayAlert("Success", "Your password has been reset!", "OK");

            // Navigate back to the Login page (PopToRootAsync is safest here)
            await Navigation.PopToRootAsync();
        }

        // Logic for the BACK button
        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Returns to the Login page (or wherever the user came from)
            await Navigation.PopAsync();
        }
    }
}