using Microsoft.Maui.Controls;
using System;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker
{
    public partial class ForgotPasswordPage : ContentPage
    {
        private readonly IAuthService _authService;

        public ForgotPasswordPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnSendResetLinkClicked(object sender, EventArgs e)
        {
            string email = EntryEmail.Text;

            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            try
            {
                // Call the Service to send emails
                await _authService.SendPasswordResetEmailAsync(email);

                await DisplayAlert("Check your email", $"We have sent a password reset link to {email}.", "OK");

                // Return to login page
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        // Logic for the BACK button
        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Returns to the Login page (or wherever the user came from)
            await Navigation.PopAsync();
        }
    }
}