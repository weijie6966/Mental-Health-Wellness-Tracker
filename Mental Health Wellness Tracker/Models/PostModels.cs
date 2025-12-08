using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class PostComment
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime CommentTime { get; set; }
        public string TimeDisplay => CommentTime.ToString("h:mm tt");
    }

    public class Post : INotifyPropertyChanged
    {
        public string Username { get; set; } = string.Empty;
        public string UserProfileImage { get; set; } = "user_icon_placeholder.png";
        public DateTime PostTime { get; set; }
        public string TimeDisplay => PostTime.ToString("dd MMM, h:mm tt");

        private string _content;
        public string Content
        {
            get => _content;
            set { if (_content != value) { _content = value; OnPropertyChanged(); } }
        }

        public string MoodEmoji { get; set; } = "emoji_neutral.png";

        public bool IsOwner
        {
            get
            {
                string currentUser = Preferences.Get("UsernameKey", "New User");
                return Username == currentUser;
            }
        }

        public ObservableCollection<ImageSource> PostImages { get; set; } = new ObservableCollection<ImageSource>();

        public bool HasImages => PostImages != null && PostImages.Count > 0;

        private int _likes;
        public int Likes
        {
            get => _likes;
            set { if (_likes != value) { _likes = value; OnPropertyChanged(); OnPropertyChanged(nameof(LikesText)); } }
        }
        public string LikesText => $"{Likes} Like{(Likes == 1 ? "" : "s")}";

        private int _commentsCount;
        public int Comments
        {
            get => _commentsCount;
            set { if (_commentsCount != value) { _commentsCount = value; OnPropertyChanged(); OnPropertyChanged(nameof(CommentsText)); } }
        }
        public string CommentsText => $"{Comments} Comment{(Comments == 1 ? "" : "s")}";

        private bool _isCommentsVisible;
        public bool IsCommentsVisible
        {
            get => _isCommentsVisible;
            set { if (_isCommentsVisible != value) { _isCommentsVisible = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<PostComment> CommentsList { get; set; } = new ObservableCollection<PostComment>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}