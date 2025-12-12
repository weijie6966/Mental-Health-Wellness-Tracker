using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    // The code-behind for ProfilePage is now clean.
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.ProfileViewModel();
        }

        // All fields, data loading, saving, and navigation logic are moved to the ViewModel.
    }
}