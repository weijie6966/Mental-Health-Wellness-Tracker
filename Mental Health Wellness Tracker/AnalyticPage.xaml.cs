using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;
using Microsoft.Maui.Storage; // Used to obtain UserID

namespace Mental_Health_Wellness_Tracker
{
    public partial class AnalyticPage : ContentPage
    {
        // Define private fields to store database repositories
        private readonly IAssessmentRepository _repository;

        // Observable collection to hold assessment history
        // Define the data collection used for binding the interface.
        public ObservableCollection<AssessmentResult> HistoryList { get; set; } = new ObservableCollection<AssessmentResult>();

        public AnalyticPage(IAssessmentRepository repository)
        {
            InitializeComponent();
            _repository = repository;

            // Set the BindingContext so that XAML can access the HistoryList.
            this.BindingContext = this;
        }

        // Triggered every time the page is displayed (better than the constructor, supports page refresh).
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadHistoryData();
        }

        // The core logic of loading history data from the database.
        private async System.Threading.Tasks.Task LoadHistoryData()
        {
            // Get current user ID
            string userId = await SecureStorage.GetAsync("user_id") ?? "unknown_user";

            // Retrieve the user's historical records from the database.
            var results = await _repository.GetAssessmentHistoryAsync(userId);

            // Clear old data and add new data
            HistoryList.Clear();
            foreach (var result in results)
            {
                HistoryList.Add(result);
            }

            // Ensure that the CollectionView on the front end uses this list.
            HistoryCollection.ItemsSource = HistoryList;
        }

        // Navigation logic for the bottom bar
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage(_repository));
            }
            else if (destination == "Diary")
            {
                await Navigation.PushAsync(new WriteDiaryPage());
            }
            else if (destination == "Profile")
            {
                await Navigation.PushAsync(new ProfilePage());
            }
            else if (destination == "List")
            {
                // Manually obtain the repository from the system service.
                var repo = IPlatformApplication.Current.Services.GetService<IAssessmentRepository>();
                // Send in the obtained repository to the AssessmentPage.
                await Navigation.PushAsync(new AssessmentPage(repo));
            }
        }
    }
}