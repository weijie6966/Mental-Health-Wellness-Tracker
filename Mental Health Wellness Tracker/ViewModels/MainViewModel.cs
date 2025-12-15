using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Views;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

        // Data Properties bound to the View
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsPasswordVisible { get; set; } = false;

        // Computed Properties (Handle visual changes)
        public string TogglePasswordImageSource => IsPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsPasswordEntryHidden => !IsPasswordVisible;

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        // REMOVED: SocialLoginCommand is deleted

        // NEW: Command to handle taps on unimplemented features (social buttons)
        public ICommand UnimplementedCommand { get; }

        public MainViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await OnLoginClicked());
            TogglePasswordCommand = new RelayCommand(OnTogglePasswordClicked);

            CreateAccountCommand = new RelayCommand(async _ => await OnNavTapped(nameof(SignUpPage)));
            ForgotPasswordCommand = new RelayCommand(async _ => await OnNavTapped(nameof(ForgotPasswordPage)));

            // NEW: Initialize the command for unimplemented features
            UnimplementedCommand = new RelayCommand(async param => await OnUnimplementedClicked(param?.ToString()));
        }

        // --- Core Authentication Logic ---

        private async Task OnLoginClicked()
        {
            if (IsBusy) return;

            IsBusy = true;

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter both email and password.", "OK");
                IsBusy = false;
                return;
            }

            // Validation uses the injected service
            if (!_authService.IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Password must contain Upper, Lower, and Number.", "OK");
                return;
            }

            try
            {
                // Login uses the injected service
                string userId = await _authService.LoginAsync(Email, Password);

                // Navigate to the next page (ProfilePage) using DI
                await OnNavTapped(nameof(ProfilePage));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Login Failed", $"Error: {ex.Message}", "Try Again");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // NEW: Logic for handling taps on unimplemented social buttons (Google, Apple, etc.)
        private async Task OnUnimplementedClicked(string featureName)
        {
            if (string.IsNullOrEmpty(featureName)) return;

            await Application.Current.MainPage.DisplayAlert(
                "Future Update",
                $"The {featureName} feature is currently in development and will be available in a future update.",
                "OK");
        }

        // --- UI Interaction Logic ---

        private void OnTogglePasswordClicked(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
            OnPropertyChanged(nameof(TogglePasswordImageSource));
            OnPropertyChanged(nameof(IsPasswordEntryHidden));
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;

            Page nextPage = destination switch
            {
                nameof(SignUpPage) => new SignUpPage(),
                nameof(ForgotPasswordPage) => new ForgotPasswordPage(),
                nameof(ProfilePage) => new ProfilePage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}