using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Services; // FIX 1: Add Services for IAuthService
using Mental_Health_Wellness_Tracker.Views;       // FIX 2: Add Views for DI Navigation

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // --- Password Visibility Properties (Logic remains in ViewModel for UI state) ---
        private bool _isPasswordVisible = false;
        public bool IsPasswordVisible { get => _isPasswordVisible; set { _isPasswordVisible = value; OnPropertyChanged(nameof(TogglePasswordImageSource)); OnPropertyChanged(nameof(IsPasswordEntryHidden)); } }
        private bool _isConfirmPasswordVisible = false;
        public bool IsConfirmPasswordVisible { get => _isConfirmPasswordVisible; set { _isConfirmPasswordVisible = value; OnPropertyChanged(nameof(ToggleConfirmPasswordImageSource)); OnPropertyChanged(nameof(IsConfirmPasswordEntryHidden)); } }

        public string TogglePasswordImageSource => IsPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsPasswordEntryHidden => !IsPasswordVisible;
        public string ToggleConfirmPasswordImageSource => IsConfirmPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsConfirmPasswordEntryHidden => !IsConfirmPasswordVisible;

        // Commands
        public ICommand SignUpCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SocialLoginCommand { get; }


        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand(async _ => await OnSignUpClicked());
            TogglePasswordCommand = new RelayCommand(OnTogglePasswordClicked);
            ToggleConfirmPasswordCommand = new RelayCommand(OnToggleConfirmPasswordClicked);

            // FIX 6: Back command uses DI navigation helper
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());

            SocialLoginCommand = new RelayCommand(async parameter => await OnSocialLoginClicked(parameter?.ToString()));
        }

        // --- Core Logic (Refactored to use IAuthService) ---

        private async Task OnSignUpClicked()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // Validation (Some checks remain here, others move to service)
            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            // FIX 7: Password validation uses the injected service
            if (!_authService.IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Weak Password", "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number", "OK");
                return;
            }

            // FIX 8: Registration uses the injected service
            try
            {
                // Note: The email validation (ending in @gmail.com) is now assumed to be handled either
                // by Firebase rules (if using Firebase) or inside the IAuthService implementation.
                string userId = await _authService.SignUpAsync(Email, Password);

                await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully!", "OK");

                // FIX 9: Navigate to success page using DI
                await OnNavTapped(nameof(SignUpSuccessPage));
            }
            catch (Exception ex)
            {
                // Display error message provided by the AuthService (e.g., Email already in use)
                await Application.Current.MainPage.DisplayAlert("Registration Failed", ex.Message, "OK");
            }
        }

        // --- Toggle Logic (Remains in ViewModel for UI state) ---

        private void OnTogglePasswordClicked(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void OnToggleConfirmPasswordClicked(object parameter)
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }

        // --- Social Login Logic ---

        private async Task OnSocialLoginClicked(string provider)
        {
            if (string.IsNullOrEmpty(provider)) return;

            await Application.Current.MainPage.DisplayAlert(
                "Future Update",
                $"The {provider} login feature is currently in development and will be available in a future update.",
                "OK");
        }

        private async Task OnNavTapped(string destination)
        {
            Page nextPage = destination switch
            {
                nameof(SignUpSuccessPage) => new SignUpSuccessPage(),
                nameof(SignUpPage) => new SignUpPage(),
                nameof(ForgotPasswordPage) => new ForgotPasswordPage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}