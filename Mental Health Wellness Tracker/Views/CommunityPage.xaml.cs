using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;
using System.Threading.Tasks;

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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            CommunityContent.Opacity = 0;
            CommunityContent.TranslationY = 20;

            await Task.WhenAll(
                CommunityContent.FadeTo(1, 250, Easing.CubicOut),
                CommunityContent.TranslateTo(0, 0, 250, Easing.CubicOut));

            _viewModel.AppearingCommand?.Execute(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
