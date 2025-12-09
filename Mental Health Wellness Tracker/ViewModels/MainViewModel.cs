using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsPasswordVisible { get; set; } = false;

        // Computed Properties (uses base.OnPropertyChanged)
        public string TogglePasswordImageSource => IsPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsPasswordEntryHidden => !IsPasswordVisible;

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand SocialLoginCommand { get; }

        public MainViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await OnLoginClicked());
            TogglePasswordCommand = new RelayCommand(OnTogglePasswordClicked);
            CreateAccountCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PushAsync(new SignUpPage()));
            ForgotPasswordCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PushAsync(new ForgotPasswordPage()));
            SocialLoginCommand = new RelayCommand(async parameter => await OnSocialLoginClicked(parameter?.ToString()));
        }

        // --- Logic (Moved from MainPage.xaml.cs) ---

        private async Task OnLoginClicked()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            if (!IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Password must contain Upper, Lower, and Number.", "OK");
                return;
            }

            string storedEmail = Preferences.Get("UserEmail", string.Empty);
            string storedPassword = Preferences.Get("UserPassword", string.Empty);

            if (string.IsNullOrEmpty(storedEmail))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No account found. Please create one.", "OK");
                return;
            }

            if (Email == storedEmail && Password == storedPassword)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new ProfilePage());
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Incorrect Credentials.", "Try Again");
            }
        }

        private void OnTogglePasswordClicked(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;

            // Notify the View that the computed properties need updating
            OnPropertyChanged(nameof(TogglePasswordImageSource));
            OnPropertyChanged(nameof(IsPasswordEntryHidden));
        }

        private async Task OnSocialLoginClicked(string provider)
        {
            await Application.Current.MainPage.DisplayAlert(provider, $"Connecting to {provider}...", "OK");
            await Task.Delay(1500);

            Preferences.Set("UserEmail", $"{provider.ToLower()}@user.com");

            await Application.Current.MainPage.DisplayAlert("Success", $"Successfully logged in with {provider}!", "OK");

            await Application.Current.MainPage.Navigation.PushAsync(new WriteDiaryPage());
        }

        private bool IsPasswordValid(string password)
        {
            return password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }
    }
}