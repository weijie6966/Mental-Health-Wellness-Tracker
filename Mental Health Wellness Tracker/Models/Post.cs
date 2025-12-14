using System;
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
        private DateTime _dateCreated;
        public DateTime DateCreated
        {
            get => _dateCreated;
            set
            {
                if (_dateCreated != value)
                {
                    _dateCreated = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TimeDisplay));
                }
            }
        }
        public string TimeDisplay => FormatRelativeTime(DateCreated);
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

        private static string FormatRelativeTime(DateTime timestamp)
        {
            if (timestamp == default)
            {
                return string.Empty;
            }

            var localTime = timestamp.Kind == DateTimeKind.Utc ? timestamp.ToLocalTime() : timestamp;
            var span = DateTime.Now - localTime;

            if (span.TotalSeconds < 60)
            {
                return "Just now";
            }

            if (span.TotalMinutes < 60)
            {
                return $"{(int)span.TotalMinutes} min ago";
            }

            if (span.TotalHours < 24)
            {
                return $"{(int)span.TotalHours} h ago";
            }

            if (span.TotalDays < 7)
            {
                return $"{(int)span.TotalDays} d ago";
            }

            return localTime.ToString("MMM d, yyyy");
        }
    }
}