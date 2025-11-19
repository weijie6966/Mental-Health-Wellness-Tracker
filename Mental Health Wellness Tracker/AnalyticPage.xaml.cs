using Microsoft.Maui.Controls;
using System;

namespace Mental_Health_Wellness_Tracker
{
    public partial class AnalyticPage : ContentPage
    {
        public AnalyticPage()
        {
            InitializeComponent();
        }

        // Navigation logic for the bottom bar
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            if (destination == "Community")
            {
                await Navigation.PushAsync(new CommunityPage());
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
                await Navigation.PushAsync(new AssessmentPage());
            }
        }
    }
}