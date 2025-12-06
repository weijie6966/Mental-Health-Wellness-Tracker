using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Mental_Health_Wellness_Tracker
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

        // --- UPDATED: SUPPORT MULTIPLE IMAGES ---
        // We now store a List of Images instead of a single one
        public ObservableCollection<ImageSource> PostImages { get; set; } = new ObservableCollection<ImageSource>();

        // Helper to hide the carousel if no images were uploaded
        public bool HasImages => PostImages != null && PostImages.Count > 0;
        // ----------------------------------------

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

    public partial class CommunityPage : ContentPage, INotifyPropertyChanged
    {
        public static ObservableCollection<Post> SharedPosts { get; set; } = new ObservableCollection<Post>();

        private Post _activePostForComment;
        private bool _isInputVisible;
        public bool IsInputVisible { get => _isInputVisible; set { _isInputVisible = value; OnPropertyChanged(nameof(IsInputVisible)); } }

        public CommunityPage(Post newEntry = null)
        {
            InitializeComponent();
            BindingContext = this;

            if (newEntry != null)
            {
                SharedPosts.Insert(0, newEntry);
            }

            PostsCollection.ItemsSource = SharedPosts;
        }

        // (Rest of logic: Delete, Edit, Comment, Like, Nav remains the same)
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Post postToDelete)
            {
                bool answer = await DisplayAlert("Delete Post", "Are you sure?", "Yes", "No");
                if (answer) SharedPosts.Remove(postToDelete);
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Post postToEdit)
            {
                string result = await DisplayPromptAsync("Edit Post", "Update text:", initialValue: postToEdit.Content);
                if (result != null) postToEdit.Content = result;
            }
        }

        private void OnCommentViewClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedPost = button?.BindingContext as Post;
            if (selectedPost == null) return;
            selectedPost.IsCommentsVisible = !selectedPost.IsCommentsVisible;
            if (selectedPost.IsCommentsVisible) { _activePostForComment = selectedPost; IsInputVisible = true; EntryCommentText.Focus(); }
            else { _activePostForComment = null; IsInputVisible = false; EntryCommentText.Unfocus(); }
        }

        private void OnCommentSendClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryCommentText.Text) || _activePostForComment == null) return;
            _activePostForComment.CommentsList.Add(new PostComment
            {
                Username = Preferences.Get("UsernameKey", "Me"),
                Text = EntryCommentText.Text,
                CommentTime = DateTime.Now
            });
            _activePostForComment.Comments++;
            EntryCommentText.Text = "";
        }

        private void OnLikeClicked(object sender, EventArgs e) { if (sender is Button b && b.CommandParameter is Post p) p.Likes++; }

        private async void OnNavTapped(object sender, EventArgs e)
        {
            string d = ((Button)sender).AutomationId;
            if (d == "List") await Navigation.PushAsync(new AssessmentPage());
            else if (d == "Diary") await Navigation.PushAsync(new WriteDiaryPage());
            else if (d == "Stats") await Navigation.PushAsync(new AnalyticPage());
            else if (d == "Profile") await Navigation.PushAsync(new ProfilePage());
        }
    }
}