using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        // Data Properties (Fody handles INPC)
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string VerificationCode { get; set; }

        // --- Visibility Toggles (Logic moved from code-behind) ---
        private bool _isNewPasswordVisible = false;
        public bool IsNewPasswordVisible
        {
            get => _isNewPasswordVisible;
            set
            {
                _isNewPasswordVisible = value;
                // Manually trigger updates for dependent properties
                OnPropertyChanged(nameof(ToggleNewPasswordImageSource));
                OnPropertyChanged(nameof(IsNewPasswordEntryHidden));
            }
        }

        private bool _isConfirmPasswordVisible = false;
        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set
            {
                _isConfirmPasswordVisible = value;
                // Manually trigger updates for dependent properties
                OnPropertyChanged(nameof(ToggleConfirmPasswordImageSource));
                OnPropertyChanged(nameof(IsConfirmPasswordEntryHidden));
            }
        }

        // Computed Properties
        public string ToggleNewPasswordImageSource => IsNewPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsNewPasswordEntryHidden => !IsNewPasswordVisible;

        public string ToggleConfirmPasswordImageSource => IsConfirmPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsConfirmPasswordEntryHidden => !IsConfirmPasswordVisible;

        // --- Commands ---
        public ICommand SendVerificationCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand ToggleNewPasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand BackCommand { get; }

        public ForgotPasswordViewModel()
        {
            SendVerificationCommand = new RelayCommand(async _ => await OnSendVerificationClicked());
            ResetPasswordCommand = new RelayCommand(async _ => await OnResetPasswordClicked());
            ToggleNewPasswordCommand = new RelayCommand(OnToggleNewPasswordClicked);
            ToggleConfirmPasswordCommand = new RelayCommand(OnToggleConfirmPasswordClicked);
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());
        }

        // --- Core Logic (Moved from ForgotPasswordPage.xaml.cs) ---

        private async Task OnSendVerificationClicked()
        {
            string inputEmail = Email?.Trim();

            if (string.IsNullOrWhiteSpace(inputEmail))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }
            if (!inputEmail.Contains("@") || !inputEmail.EndsWith("@gmail.com"))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Email", "Please use a valid Google account (must end in lowercase @gmail.com).", "OK");
                return;
            }

            string storedEmail = Preferences.Get("UserEmail", string.Empty);
            if (inputEmail != storedEmail)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "This email is not registered. Please create a new account.", "OK");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Sent", $"Verification code sent to {inputEmail}", "OK");
        }

        private async Task OnResetPasswordClicked()
        {
            string email = Email?.Trim();
            string newPass = NewPassword;
            string confirmPass = ConfirmPassword;
            string code = VerificationCode;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass) || string.IsNullOrEmpty(code))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            string storedEmail = Preferences.Get("UserEmail", string.Empty);
            if (email != storedEmail)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Email does not match our records.", "OK");
                return;
            }

            if (newPass != confirmPass)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New passwords do not match.", "OK");
                return;
            }

            if (!IsPasswordValid(newPass))
            {
                await Application.Current.MainPage.DisplayAlert("Weak Password",
                    "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number",
                    "OK");
                return;
            }

            // SUCCESS: Overwrite the old password
            Preferences.Set("UserPassword", newPass);

            await Application.Current.MainPage.DisplayAlert("Success", "Your password has been reset! You can now log in.", "OK");

            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        private bool IsPasswordValid(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }

        private void OnToggleNewPasswordClicked(object parameter)
        {
            IsNewPasswordVisible = !IsNewPasswordVisible;
        }

        private void OnToggleConfirmPasswordClicked(object parameter)
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }
    }
}