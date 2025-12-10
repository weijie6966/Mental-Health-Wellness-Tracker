using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using Mental_Health_Wellness_Tracker.Views;
using System;

namespace Mental_Health_Wellness_Tracker
{
    public partial class App : Application
    {
        // FIX 1: Add a private field to store the service provider
        private readonly IServiceProvider _serviceProvider;

        // FIX 2: Create a constructor that accepts the IServiceProvider via DI.
        // The MAUI framework automatically calls this constructor in MauiProgram.cs.
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider; // Store the service provider

            // FIX 3: Resolve the starting page using the stored service provider
            // GetRequiredService ensures the MainPage (and its MainViewModel dependency) is created.
            // Ensure you are using the fully qualified name or the correct using for MainPage.
            MainPage startPage = _serviceProvider.GetRequiredService<MainPage>();

            // Wrap MainPage in a NavigationPage
            MainPage = new NavigationPage(startPage);
        }
    }
}