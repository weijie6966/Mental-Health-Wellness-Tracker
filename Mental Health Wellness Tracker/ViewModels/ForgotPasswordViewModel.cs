using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

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

            try
            {
                await _authService.SendPasswordResetEmailAsync(inputEmail);
                await Application.Current.MainPage.DisplayAlert("Sent", $"Password reset link sent to {inputEmail}. Please check your inbox.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task OnResetPasswordClicked()
        {
            string email = Email?.Trim();
            string newPass = NewPassword;
            string confirmPass = ConfirmPassword;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (newPass != confirmPass)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New passwords do not match.", "OK");
                return;
            }

            if (!_authService.IsPasswordValid(newPass))
            {
                await Application.Current.MainPage.DisplayAlert("Weak Password",
                    "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number",
                    "OK");
                return;
            }

            try
            {
                await _authService.SendPasswordResetEmailAsync(email);
                await Application.Current.MainPage.DisplayAlert("Check Your Email", "We sent you a reset link. Follow it to set your new password.", "OK");
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
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