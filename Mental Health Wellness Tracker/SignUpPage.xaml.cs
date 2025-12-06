using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker
{
    public partial class SignUpPage : ContentPage
    {
        private bool _isPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public SignUpPage()
        {
            InitializeComponent();
        }

        // --- SOCIAL SIGN UP LOGIC ---
        private async void OnGoogleLoginClicked(object sender, EventArgs e) => await PerformSocialLogin("Google");
        private async void OnFacebookLoginClicked(object sender, EventArgs e) => await PerformSocialLogin("Facebook");
        private async void OnAppleLoginClicked(object sender, EventArgs e) => await PerformSocialLogin("Apple");

        private async Task PerformSocialLogin(string provider)
        {
            await DisplayAlert(provider, $"Connecting to {provider}...", "OK");
            await Task.Delay(1500); // Simulate connection

            // Set dummy account for testing
            Preferences.Set("UserEmail", $"{provider.ToLower()}@user.com");

            await DisplayAlert("Success", $"Account created with {provider}!", "OK");
            await Navigation.PushAsync(new SignUpSuccessPage());
        }
        // -----------------------------

        private async void OnSignUpClicked(object sender, EventArgs e)
        {
            string email = EntryEmail.Text?.Trim();
            string password = EntryPassword.Text;
            string confirmPass = EntryConfirmPassword.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPass))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (!email.Contains("@") || !email.EndsWith("@gmail.com"))
            {
                await DisplayAlert("Invalid Email", "Please use a valid Google account (must end in lowercase @gmail.com).", "OK");
                return;
            }

            if (password != confirmPass)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (!IsPasswordValid(password))
            {
                await DisplayAlert("Weak Password", "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number", "OK");
                return;
            }

            Preferences.Set("UserEmail", email);
            Preferences.Set("UserPassword", password);

            await DisplayAlert("Success", "Account created successfully!", "OK");
            await Navigation.PushAsync(new SignUpSuccessPage());
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

        private void OnToggleConfirmPasswordClicked(object sender, EventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
            EntryConfirmPassword.IsPassword = !_isConfirmPasswordVisible;
            BtnToggleConfirmPassword.Source = _isConfirmPasswordVisible ? "eye_closed.png" : "eye_open.png";
        }

        private async void OnBackArrowClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}