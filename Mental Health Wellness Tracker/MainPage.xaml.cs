using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Linq;
using System.Threading.Tasks; // Needed for Task.Delay

namespace Mental_Health_Wellness_Tracker
{
    public partial class MainPage : ContentPage
    {
        private bool _isPasswordVisible = false;

        public MainPage()
        {
            InitializeComponent();
        }

        // --- SOCIAL LOGIN LOGIC ---

        private async void OnGoogleLoginClicked(object sender, EventArgs e)
        {
            await PerformSocialLogin("Google");
        }

        private async void OnFacebookLoginClicked(object sender, EventArgs e)
        {
            await PerformSocialLogin("Facebook");
        }

        private async void OnAppleLoginClicked(object sender, EventArgs e)
        {
            await PerformSocialLogin("Apple");
        }

        // Helper method to simulate the connection process
        private async Task PerformSocialLogin(string provider)
        {
            // 1. Show "Connecting..." message
            await DisplayAlert(provider, $"Connecting to {provider}...", "OK");

            // 2. Simulate network delay (1.5 seconds)
            await Task.Delay(1500);

            // 3. Simulate success
            // In a real app with backend, you would get a token here.
            // For now, we save a mock user session.
            Preferences.Set("UserEmail", $"{provider.ToLower()}@user.com");

            await DisplayAlert("Success", $"Successfully logged in with {provider}!", "OK");

            // 4. Navigate to Diary
            await Navigation.PushAsync(new WriteDiaryPage());
        }
        // --------------------------

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EntryEmail.Text?.Trim();
            string password = EntryPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            if (!IsPasswordValid(password))
            {
                await DisplayAlert("Invalid Input", "Password must contain Upper, Lower, and Number.", "OK");
                return;
            }

            string storedEmail = Preferences.Get("UserEmail", string.Empty);
            string storedPassword = Preferences.Get("UserPassword", string.Empty);

            if (string.IsNullOrEmpty(storedEmail))
            {
                await DisplayAlert("Error", "No account found. Please create one.", "OK");
                return;
            }

            if (email == storedEmail && password == storedPassword)
            {
                await Navigation.PushAsync(new WriteDiaryPage());
            }
            else
            {
                await DisplayAlert("Error", "Incorrect Credentials.", "Try Again");
            }
        }

        private bool IsPasswordValid(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }

        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            EntryPassword.IsPassword = !_isPasswordVisible;
            BtnTogglePassword.Source = _isPasswordVisible ? "eye_closed.png" : "eye_open.png";
        }

        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}