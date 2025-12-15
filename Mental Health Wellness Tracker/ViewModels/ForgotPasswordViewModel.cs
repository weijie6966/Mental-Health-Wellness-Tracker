using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

        // Data Properties (Fody handles INPC)
        public string Email { get; set; }

        // --- Commands ---
        public ICommand SendVerificationCommand { get; }
        public ICommand BackCommand { get; }

        public ForgotPasswordViewModel()
        {
            SendVerificationCommand = new RelayCommand(async _ => await OnSendVerificationClicked());
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());
        }

        // --- Core Logic (Moved from ForgotPasswordPage.xaml.cs) ---

        private async Task OnSendVerificationClicked()
        {
            if (IsBusy) return;

            IsBusy = true;

            string inputEmail = Email?.Trim();

            if (string.IsNullOrWhiteSpace(inputEmail))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter your email address.", "OK");
                IsBusy = false;
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
            finally
            {
                IsBusy = false;
            }
        }
    }
}