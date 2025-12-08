namespace Mental_Health_Wellness_Tracker;

public partial class SignUpSuccessPage : ContentPage
{
    public SignUpSuccessPage()
    {
        InitializeComponent();
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        // PopToRootAsync removes all pages on top of the Root (Login) page.
        // This effectively returns the user to the Login screen.
        await Navigation.PopToRootAsync();
    }
}