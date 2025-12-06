using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // Needed for Preferences
using System;
using System.Linq; // Needed for password complexity logic

namespace Mental_Health_Wellness_Tracker
{
    public partial class ForgotPasswordPage : ContentPage
    {
        private bool _isNewPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        // --- 1. SEND VERIFICATION LOGIC ---
        private async void OnSendVerificationClicked(object sender, EventArgs e)
        {
            string inputEmail = EntryEmail.Text?.Trim();

            // A. Basic Empty Check
            if (string.IsNullOrWhiteSpace(inputEmail))
            {
                await DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            // B. STRICT GMAIL FORMAT CHECK (Same as SignUp)
            if (!inputEmail.Contains("@") || !inputEmail.EndsWith("@gmail.com"))
            {
                await DisplayAlert("Invalid Email", "Please use a valid Google account (must end in lowercase @gmail.com).", "OK");
                return;
            }

            // C. Check if account exists in system
            string storedEmail = Preferences.Get("UserEmail", string.Empty);

            if (inputEmail != storedEmail)
            {
                await DisplayAlert("Error", "This email is not registered. Please create a new account.", "OK");
                return;
            }

            // D. Simulate sending code
            await DisplayAlert("Sent", $"Verification code sent to {inputEmail}", "OK");
        }

        // --- 2. RESET PASSWORD LOGIC ---
        private async void OnResetPasswordClicked(object sender, EventArgs e)
        {
            string email = EntryEmail.Text?.Trim();
            string newPass = EntryNewPassword.Text;
            string confirmPass = EntryConfirmPassword.Text;
            string code = EntryVerificationCode.Text;

            // A. Basic Validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass) || string.IsNullOrEmpty(code))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // B. Verify Email Matches Saved Account again (Safety check)
            string storedEmail = Preferences.Get("UserEmail", string.Empty);
            if (email != storedEmail)
            {
                await DisplayAlert("Error", "Email does not match our records.", "OK");
                return;
            }

            // C. Verify Passwords Match
            if (newPass != confirmPass)
            {
                await DisplayAlert("Error", "New passwords do not match.", "OK");
                return;
            }

            // D. VERIFY PASSWORD COMPLEXITY (Same as SignUp)
            if (!IsPasswordValid(newPass))
            {
                await DisplayAlert("Weak Password",
                    "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number",
                    "OK");
                return;
            }

            // E. SUCCESS: Overwrite the old password
            Preferences.Set("UserPassword", newPass);

            await DisplayAlert("Success", "Your password has been reset! You can now log in.", "OK");

            // Return to Login Page
            await Navigation.PopToRootAsync();
        }

        // Helper: Check Password Rules
        private bool IsPasswordValid(string password)
        {
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasNumber = password.Any(char.IsDigit);
            return hasUpper && hasLower && hasNumber;
        }

        // --- TOGGLE BUTTONS ---
        private void OnToggleNewPasswordClicked(object sender, EventArgs e)
        {
            _isNewPasswordVisible = !_isNewPasswordVisible;
            EntryNewPassword.IsPassword = !_isNewPasswordVisible;
            BtnToggleNewPassword.Source = _isNewPasswordVisible ? "eye_closed.png" : "eye_open.png";
        }

        private void OnToggleConfirmPasswordClicked(object sender, EventArgs e)
        {
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
            EntryConfirmPassword.IsPassword = !_isConfirmPasswordVisible;
            BtnToggleConfirmPassword.Source = _isConfirmPasswordVisible ? "eye_closed.png" : "eye_open.png";
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}