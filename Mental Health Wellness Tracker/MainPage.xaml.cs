using Microsoft.Maui.Controls;
using System;
using Mental_Health_Wellness_Tracker.Services;


namespace Mental_Health_Wellness_Tracker
{
    public partial class MainPage : ContentPage
    {
        // Variable to track if the password is visible or hidden
        private bool _isPasswordVisible = false;

        // Define a private read-only field to store the service.
        private readonly IAuthService _authService;

        // Constructor with dependency injection
        // The IAuthService instance will be provided by the DI container.
        public MainPage(IAuthService authService)
        {
            InitializeComponent();

            // Assign the injected service to the private field
            _authService = authService;
        }

        // 1. Login Button Logic (Navigates to Diary Page)
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // --- Basic Logic ---
            // In a real application, you would validate the username/password against a database here.
            string email = EntryEmail.Text;
            string password = EntryPassword.Text;

            // Basic validation: Check if it is empty
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            try
            {
                // Call AuthService to perform the actual login.
                // The _authService here is the one we injected through the constructor.
                string userId = await _authService.LoginAsync(email, password);

                // Login successful! The user will be notified and redirected.
                await DisplayAlert("Success", "Login successful!", "OK");

                // Jump to the ProfilePage
                await Navigation.PushAsync(new ProfilePage());
            }
            catch (Exception ex)
            {
                // Login failed, show an error message
                await DisplayAlert("Login Failed", ex.Message, "OK");
            }
        }

        // 2. Show/Hide Password Logic (Eye Icon)
        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            // Toggle the state
            _isPasswordVisible = !_isPasswordVisible;

            // Update the Entry property (EntryPassword is named in the XAML)
            EntryPassword.IsPassword = !_isPasswordVisible;

            // Swap the image source based on the state
            if (_isPasswordVisible)
            {
                BtnTogglePassword.Source = "eye_closed.png";
            }
            else
            {
                BtnTogglePassword.Source = "eye_open.png";
            }
        }

        // 3. Create New Account Logic (Footer Link)
        private async void OnCreateAccountClicked(object sender, EventArgs e)
        {
            // Navigate to the Sign Up Page
            await Navigation.PushAsync(new SignUpPage(_authService));
        }

        // 4. Forgot Password Logic (Footer Link)
        private async void OnForgotPasswordClicked(object sender, EventArgs e)
        {
            // Navigate to the Forgot Password Recovery Page
            await Navigation.PushAsync(new ForgotPasswordPage(_authService));
        }
    }
}