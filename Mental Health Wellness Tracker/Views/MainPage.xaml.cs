using Mental_Health_Wellness_Tracker.ViewModels;
using Microsoft.Maui.Controls;

// Assuming your pages are in the Views namespace
namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class MainPage : ContentPage
    {
        // FIX 1: Remove the parameterless constructor.
        // FIX 2: Accept the ViewModel via Dependency Injection (DI).
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            // FIX 3: Set the BindingContext to the injected instance.
            this.BindingContext = viewModel;
        }

        // NOTE: You must also remove the <ContentPage.BindingContext> tag from MainPage.xaml
        // if you had one, as the BindingContext is now set entirely in the C# code-behind.
    }
}