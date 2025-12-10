using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;
using Microsoft.Extensions.DependencyInjection; // Essential for DI service locator (fallback)
using System; // For Application.Current

// NOTE: We assume the XAML file has x:Class="Mental_Health_Wellness_Tracker.Views.CommunityPage"
namespace Mental_Health_Wellness_Tracker.Views
{
    // FIX: Ensure the 'partial' keyword is present here!
    public partial class CommunityPage : ContentPage, INotifyPropertyChanged
    {
        // FIX 1: Constructor uses Dependency Injection to get the ViewModel
        public CommunityPage(CommunityViewModel viewModel)
        {
            // This call should now link to the auto-generated code.
            InitializeComponent();

            // Set the BindingContext to the injected ViewModel
            this.BindingContext = viewModel;

            // ... (rest of the logic for the default constructor)
        }

        // Overload constructor to accept a new Post model during navigation
        public CommunityPage(Post newEntry) : this(GetViewModelFromDI())
        {
            // InitializeComponent is implicitly called via the : this(GetViewModelFromDI()) constructor chain

            if (newEntry != null)
            {
                (this.BindingContext as CommunityViewModel)?.AddNewPost(newEntry);
            }
        }

        // Helper to retrieve ViewModel from DI for constructor overloads
        private static CommunityViewModel GetViewModelFromDI()
        {
            // FIX: Use optional chaining or null checks for robustness
            return Application.Current?.Handler?.MauiContext?.Services?.GetService<CommunityViewModel>();
        }

        // --- Clean up INPC boilerplate (Remove if ViewModel handles all binding logic) ---
        // If your XAML binds only to the ViewModel, you don't need INPC on the page itself.
        // However, if you are forced to keep it due to structure:
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}