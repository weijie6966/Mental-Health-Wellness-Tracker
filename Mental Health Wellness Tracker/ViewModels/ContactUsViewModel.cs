using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class ContactUsViewModel : ViewModelBase
    {
        // --- CONTACT INFORMATION ---
        public string CounsellorName => "Kevin Wong, UTS Counsellor";
        public string EmailAddress => "kevin@uts.edu.my";
        public string SecondaryEmailAddress => "counseling.psy@uts.edu.my";
        public string PhoneNumber => "+601156883700";
        public string AddressLine1 => "1, Jalan University";
        public string AddressLine2 => "96000 Sibu, Sarawak";
        public string LinktreeUrl => "https://linktr.ee/counseloruts?utm_source=qr_code";
        public string MapUrl => "https://maps.app.goo.gl/EssGPhWEfCLvzXy58";

        // NEW: Gmail URL property
        public string GmailUrl => "https://mail.google.com";

        // Commands
        public ICommand BackCommand { get; }
        public ICommand OpenLinkCommand { get; }
        // Removed: Email1TappedCommand and Email2TappedCommand
        public ICommand CallPhoneCommand { get; }
        public ICommand OpenMapCommand { get; }

        public ContactUsViewModel()
        {
            BackCommand = new RelayCommand(async _ => await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PopAsync());
            OpenLinkCommand = new RelayCommand(async url => await OpenLink(url?.ToString()));

            // Removed: Email Tapped command initializations
            OpenMapCommand = new RelayCommand(async _ => await OpenMapLocation());
        }

        // --- EXECUTION LOGIC ---

        private async Task OpenMapLocation()
        {
            await OpenLink(MapUrl);
        }

        private async Task OpenLink(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                try
                {
                    await Launcher.Default.OpenAsync(uri);
                }
                catch (Exception ex)
                {
                    await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", $"Could not open link: {ex.Message}", "OK");
                }
            }
            else
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", "Invalid link format.", "OK");
            }
        }
    }
}