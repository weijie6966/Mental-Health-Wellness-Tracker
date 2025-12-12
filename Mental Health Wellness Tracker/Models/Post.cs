using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models; // For PostComment reference

namespace Mental_Health_Wellness_Tracker.Models
{
    public class Post : INotifyPropertyChanged
    {
        // Data Fields
        public string FirestoreId { get; set; } // Cloud document id for edit/delete actions
        public string UserId { get; set; } // Added for deletion/editing checks
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MoodEmoji { get; set; } = "emoji_neutral.png";

        // Social Fields
        private int _likes;
        public int Likes
        {
            get => _likes;
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LikesText));
                }
            }
        }
        public string LikesText => $"{Likes} Like{(Likes == 1 ? "" : "s")}";

        private int _commentsCount;
        public int Comments
        {
            get => _commentsCount;
            set
            {
                if (_commentsCount != value)
                {
                    _commentsCount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CommentsText));
                }
            }
        }
        public string CommentsText => $"{Comments} Comment{(Comments == 1 ? "" : "s")}";

        private bool _isCommentsVisible;
        public bool IsCommentsVisible
        {
            get => _isCommentsVisible;
            set
            {
                if (_isCommentsVisible != value)
                {
                    _isCommentsVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<PostComment> CommentsList { get; set; } = new ObservableCollection<PostComment>();

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}