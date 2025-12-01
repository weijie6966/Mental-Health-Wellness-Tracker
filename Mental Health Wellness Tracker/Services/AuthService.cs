using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Maui.Storage; // Used to store tokens

namespace Mental_Health_Wellness_Tracker.Services
{
    public class AuthService : IAuthService
    {
        // IMPORTANT: Please replace the string below with your "Web API Key" in the Firebase console.
        private const string WebApiKey = "AIzaSyARXVMSRY2JvzMJue2jWUoCd44bv1TYBaE";

        private FirebaseAuthClient _authClient;

        public AuthService()
        {
            // Configure Firebase Auth
            var config = new FirebaseAuthConfig
            {
                ApiKey = WebApiKey,
                AuthDomain = "mental-health-wellness-tracker.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                {
                    // Add email/password provider
                    new EmailProvider()
                }
            };

            // Initialize Client
            _authClient = new FirebaseAuthClient(config);
        }

        public async Task<string> SignUpAsync(string email, string password)
        {
            try
            {
                // Create user
                var userCredential = await _authClient.CreateUserWithEmailAndPasswordAsync(email, password);

                // Get ID token
                var token = await userCredential.User.GetIdTokenAsync();

                // Save user session
                await SaveUserSession(token, userCredential.User.Uid, email);

                return userCredential.User.Uid;
            }
            catch (Firebase.Auth.FirebaseAuthHttpException ex)
            {
                // Fix for SignUp as well
                string responseData = ex.ResponseData;
                if (!string.IsNullOrEmpty(responseData) && responseData.Contains("EMAIL_EXISTS"))
                {
                    throw new Exception("This email is already registered. Please log in.");
                }
                throw new Exception($"Sign up failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Sign up failed: {ex.Message}");
            }
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                // Log in using _authClient
                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                // Get fresh auth token
                var token = await userCredential.User.GetIdTokenAsync();

                // Save user session
                await SaveUserSession(token, userCredential.User.Uid, email);

                return userCredential.User.Uid;
            }
            // Specifically designed to capture Firebase network anomalies
            catch (Exception ex)
            {
                // Retrieve the raw response data (which contains phrases such as INVALID_LOGIN_CREDENTIALS).
                string friendlyMessage = $"Login failed: {ex.Message}";

                // Check keywords
                if (ex is Firebase.Auth.FirebaseAuthHttpException fbEx)
                {
                    // If so, we have the right to view the ResponseData (that JSON string).
                    string responseData = fbEx.ResponseData;

                    if (!string.IsNullOrEmpty(responseData))
                    {
                        // 3. Check keywords
                        if (responseData.Contains("INVALID_LOGIN_CREDENTIALS") ||
                            responseData.Contains("INVALID_PASSWORD") ||
                            responseData.Contains("EMAIL_NOT_FOUND"))
                        {
                            friendlyMessage = "Incorrect email or password. Please check and try again.";
                        }
                        else if (responseData.Contains("INVALID_EMAIL"))
                        {
                            friendlyMessage = "Invalid email format. Please enter a valid email address.";
                        }
                        else if (responseData.Contains("USER_DISABLED"))
                        {
                            friendlyMessage = "This account has been disabled.";
                        }
                        else if (responseData.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
                        {
                            friendlyMessage = "Too many failed attempts. Please try again later.";
                        }
                    }
                }

                throw new Exception(friendlyMessage);
            }
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                await _authClient.ResetEmailPasswordAsync(email);
            }
            catch (Exception ex)
            {
                string friendlyMessage = $"Failed to send reset email: {ex.Message}";

                if (ex is Firebase.Auth.FirebaseAuthHttpException fbEx)
                {
                    string responseData = fbEx.ResponseData;

                    if (!string.IsNullOrEmpty(responseData))
                    {
                        // Check for specific error keywords
                        if (responseData.Contains("INVALID_EMAIL"))
                        {
                            friendlyMessage = "Invalid email format. Please check your email address.";
                        }
                        else if (responseData.Contains("EMAIL_NOT_FOUND"))
                        {
                            friendlyMessage = "This email is not registered.";
                        }
                    }
                }

                throw new Exception(friendlyMessage);
            }
        }

        public void SignOut()
        {
            // Clear stored tokens to sign out
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");
        }

        public async Task<bool> IsSignedInAsync()
        {
            // Check if the auth token exists in secure storage
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        // Private helper method: Save user session data to secure storage
        private async Task SaveUserSession(string token, string userId, string email)
        {
            await SecureStorage.SetAsync("auth_token", token);
            await SecureStorage.SetAsync("user_id", userId);
            await SecureStorage.SetAsync("user_email", email);
        }
    }
}