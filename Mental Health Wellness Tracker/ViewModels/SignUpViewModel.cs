using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        private bool _isPasswordVisible = false;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                // Manually notify when a dependent property changes
                OnPropertyChanged(nameof(TogglePasswordImageSource));
                OnPropertyChanged(nameof(IsPasswordEntryHidden));
            }
        }

        private bool _isConfirmPasswordVisible = false;
        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set
            {
                _isConfirmPasswordVisible = value;
                // Manually notify when a dependent property changes
                OnPropertyChanged(nameof(ToggleConfirmPasswordImageSource));
                OnPropertyChanged(nameof(IsConfirmPasswordEntryHidden));
            }
        }

        // Computed Properties for the View
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
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());
            SocialLoginCommand = new RelayCommand(async parameter => await OnSocialLoginClicked(parameter?.ToString()));
        }

        // --- Logic (Moved from SignUpPage.xaml.cs) ---

        private async Task OnSignUpClicked()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (!Email.Contains("@") || !Email.EndsWith("@gmail.com"))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Email", "Please use a valid Google account (must end in lowercase @gmail.com).", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            if (!IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Weak Password", "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number", "OK");
                return;
            }

            Preferences.Set("UserEmail", Email);
            Preferences.Set("UserPassword", Password);

            await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully!", "OK");
            await Application.Current.MainPage.Navigation.PushAsync(new SignUpSuccessPage());
        }

        private void OnTogglePasswordClicked(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void OnToggleConfirmPasswordClicked(object parameter)
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }

        private async Task OnSocialLoginClicked(string provider)
        {
            await Application.Current.MainPage.DisplayAlert(provider, $"Connecting to {provider}...", "OK");
            await Task.Delay(1500);

            Preferences.Set("UserEmail", $"{provider.ToLower()}@user.com");

            await Application.Current.MainPage.DisplayAlert("Success", $"Account created with {provider}!", "OK");
            await Application.Current.MainPage.Navigation.PushAsync(new SignUpSuccessPage());
        }

        private bool IsPasswordValid(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }
    }
}