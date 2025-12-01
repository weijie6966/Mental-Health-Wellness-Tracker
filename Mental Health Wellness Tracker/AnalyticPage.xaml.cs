using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // Used to obtain UserID
using System;
using System.Collections.ObjectModel;

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

            // Connect the list (HistoryCollection) on the interface to the data source (HistoryList)
            HistoryCollection.ItemsSource = HistoryList;
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
            if (string.IsNullOrEmpty(userId))
                return; // Security check

            // First, try syncing the latest data from the cloud (Fire and Forget, without blocking the UI)
            // We won't await it; let it run in the background so users can immediately see the locally cached data.
            _ = Task.Run(async () =>
            {
                await _repository.SyncAssessmentFromCloudAsync(userId);

                // After synchronization is complete, the UI should be refreshed on the main thread.
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    // Reread the latest full list from the database
                    await ReloadLocalList(userId);
                });
            });

            // Display existing local data immediately(guaranteed to open in seconds).
            await ReloadLocalList(userId);


            //// Retrieve the user's historical records from the database.
            //var results = await _repository.GetAssessmentHistoryAsync(userId);

            //// Clear old data and add new data
            //HistoryList.Clear();
            //foreach (var result in results)
            //{
            //    HistoryList.Add(result);
            //}

            //// Ensure that the CollectionView on the front end uses this list.
            //HistoryCollection.ItemsSource = HistoryList;
        }

        // Extracting the logic for reading from the local database makes it easier to reuse
        private async Task ReloadLocalList (string userId)
        {
            var results = await _repository.GetAssessmentHistoryAsync(userId);
            HistoryList.Clear();
            foreach (var result in results)
            {
                HistoryList.Add(result);
            }
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