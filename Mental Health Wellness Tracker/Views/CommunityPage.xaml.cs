using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class CommunityPage : ContentPage, INotifyPropertyChanged
    {
        private readonly CommunityViewModel _viewModel;

        public CommunityPage()
        {
            InitializeComponent();
            _viewModel = new CommunityViewModel();
            BindingContext = _viewModel;
        }

        public CommunityPage(Post newEntry) : this()
        {
            if (newEntry != null)
            {
                _viewModel.AddNewPost(newEntry);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
