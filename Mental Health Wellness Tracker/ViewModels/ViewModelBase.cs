using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // Base class for all ViewModels.
    // We explicitly implement INotifyPropertyChanged so we can define the OnPropertyChanged helper.
    // Fody handles automatic notification for properties without explicit backing fields.
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // Explicitly defining the event as part of the INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        // Helper method to raise the event manually, e.g., for computed properties.
        // The [CallerMemberName] attribute allows the method to determine the calling property's name.
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}