using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace Mental_Health_Wellness_Tracker.Services
{
    public class NotificationService
    {
        private readonly (int Id, string Title, string Description, TimeSpan TimeOfDay)[] _dailySlots = new[]
        {
            (Id: 1001, Title: "Morning check-in", Description: "Take a moment to set your intention for the day.", TimeOfDay: new TimeSpan(8, 0, 0)),
            (Id: 1002, Title: "Afternoon reflection", Description: "Pause, breathe, and see how you're feeling.", TimeOfDay: new TimeSpan(13, 0, 0)),
            (Id: 1003, Title: "Evening unwind", Description: "Wind down and note a thought or gratitude.", TimeOfDay: new TimeSpan(20, 0, 0))
        };

        public async Task InitializeAsync()
        {
            // Request permission only if not already granted, then configure the recurring reminders.
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                var userConsented = await AskForPermissionAsync();

                if (userConsented)
                {
                    await LocalNotificationCenter.Current.RequestNotificationPermission();
                }
                else
                {
                    return;
                }
            }

            ScheduleDailyNotifications();
        }

        private async Task<bool> AskForPermissionAsync()
        {
            var mainPage = Application.Current?.MainPage;

            if (mainPage == null)
            {
                return false;
            }

            return await mainPage.DisplayAlert(
                "Enable reminders?",
                "Allow daily notifications so we can remind you to check in with yourself during the day.",
                "Allow",
                "Not now");
        }

        private void ScheduleDailyNotifications()
        {
            foreach (var slot in _dailySlots)
            {
                LocalNotificationCenter.Current.Cancel(slot.Id);

                var notifyTime = DateTime.Today.Add(slot.TimeOfDay);
                if (notifyTime <= DateTime.Now)
                {
                    notifyTime = notifyTime.AddDays(1);
                }

                var request = new NotificationRequest
                {
                    NotificationId = slot.Id,
                    Title = slot.Title,
                    Description = slot.Description,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime,
                        RepeatType = NotificationRepeat.Daily
                    }
                };

                LocalNotificationCenter.Current.Show(request);
            }
        }
    }
}
