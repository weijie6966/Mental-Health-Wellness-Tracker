using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models; // For PostComment reference

namespace Mental_Health_Wellness_Tracker.Models
{
    public class Post : INotifyPropertyChanged
    {
        public Post()
        {
            PostImages.CollectionChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(PostImages));
                OnPropertyChanged(nameof(HasImages));
            };
        }

        // Data Fields
        public string FirestoreId { get; set; } // Cloud document id for edit/delete actions
        public string UserId { get; set; } // Added for deletion/editing checks
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MoodEmoji { get; set; } = "emoji_neutral.png";
        public string UserProfileImage { get; set; } = "nav_profile.png";
        private bool _isOwner;
        public bool IsOwner
        {
            get => _isOwner;
            set
            {
                if (_isOwner != value)
                {
                    _isOwner = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> PostImages { get; set; } = new ObservableCollection<string>();

        public bool HasImages => PostImages?.Count > 0;

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}