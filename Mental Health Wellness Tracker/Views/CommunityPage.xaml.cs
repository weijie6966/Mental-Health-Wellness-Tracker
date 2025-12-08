using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker
{
    // Note: PostComment and Post definitions are now found in Models/PostModels.cs

    public partial class CommunityPage : ContentPage, INotifyPropertyChanged
    {
        // The core data logic and properties are now managed by the ViewModel.

        public CommunityPage(Post newEntry = null)
        {
            InitializeComponent();

            // Instantiate the ViewModel
            var viewModel = new CommunityViewModel();
            this.BindingContext = viewModel;

            // SPECIAL CASE: Handle new post injection from WriteDiaryPage
            if (newEntry != null)
            {
                // Add the new post directly to the static ViewModel collection
                CommunityViewModel.SharedPosts.Insert(0, newEntry);
            }

            // All event handlers removed. Logic is handled by commands in the ViewModel.
        }

        // Retain INPC boilerplate if the page itself needs to notify, though less critical now.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}