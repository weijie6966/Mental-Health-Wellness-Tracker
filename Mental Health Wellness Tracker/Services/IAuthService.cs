using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker.Services
{
    public interface IAuthService
    {
        // Registration: Successfully returned UserId
        Task<string> SignUpAsync(string email, string password);

        // Login: Successfully returned UserId
        Task<string> LoginAsync(string email, string password);

        // Logout
        void SignOut();

        // Check if currently logged in
        Task<bool> IsSignedInAsync();

        // Send password reset email
        Task SendPasswordResetEmailAsync(string email);
    }
}
