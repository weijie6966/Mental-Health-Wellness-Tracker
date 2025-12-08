using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    // The code-behind for ProfilePage is now clean.
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            // All data loading and logic are handled by the ProfileViewModel.
            // The BindingContext is set in ProfilePage.xaml
        }

        // All fields, data loading, saving, and navigation logic are moved to the ViewModel.
    }
}