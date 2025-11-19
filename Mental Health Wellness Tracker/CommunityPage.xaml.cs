using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mental_Health_Wellness_Tracker
{
    // 1. COMMENT MODEL (New class for individual comments)
    public class PostComment
    {
        public string Username { get; set; }
        public string Text { get; set; }
    }

    // 2. POST MODEL (Updated to support your XAML bindings)
    public class Post : INotifyPropertyChanged
    {
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MoodEmoji { get; set; } = "emoji_neutral.png";

        // -- LIKES --
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
                    OnPropertyChanged(nameof(LikesText)); // Update the text when number changes
                }
            }
        }
        // Returns "5 Likes" or "1 Like"
        public string LikesText => $"{Likes} Like{(Likes == 1 ? "" : "s")}";

        // -- COMMENTS COUNT --
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
                    OnPropertyChanged(nameof(CommentsText)); // Update the text when number changes
                }
            }
        }
        // Returns "3 Comments"
        public string CommentsText => $"{Comments} Comment{(Comments == 1 ? "" : "s")}";

        // -- COMMENTS VISIBILITY --
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

        // -- LIST OF COMMENTS --
        public ObservableCollection<PostComment> CommentsList { get; set; } = new ObservableCollection<PostComment>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // 3. PAGE LOGIC
    public partial class CommunityPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<Post> Posts { get; set; }

        // Track which post the user is currently trying to comment on
        private Post _activePostForComment;

        // Property to show/hide the bottom input bar (referenced in XAML Row 1)
        private bool _isInputVisible;
        public bool IsInputVisible
        {
            get => _isInputVisible;
            set
            {
                _isInputVisible = value;
                OnPropertyChanged(nameof(IsInputVisible));
            }
        }

        public CommunityPage(Post newEntry = null)
        {
            InitializeComponent();
            BindingContext = this; // Important! Allows the Page to bind to IsInputVisible

            // Initialize Sample Data with Comments
            Posts = new ObservableCollection<Post>
            {
                new Post
                {
                    Username = "SarahM",
                    Content = "Feeling much better today after my morning walk!",
                    MoodEmoji = "emoji_love.png",
                    Likes = 5,
                    Comments = 1,
                    CommentsList = new ObservableCollection<PostComment>
                    {
                        new PostComment { Username = "JohnD", Text = "Great job! Keep it up." }
                    }
                },
                new Post
                {
                    Username = "Mike22",
                    Content = "Anxiety is high today. Breathing exercises aren't working.",
                    MoodEmoji = "emoji_sad.png",
                    Likes = 2,
                    Comments = 0
                }
            };

            if (newEntry != null) Posts.Insert(0, newEntry);
            PostsCollection.ItemsSource = Posts;
        }

        // --- FIX FOR THE ERROR: VIEW COMMENTS CLICKED ---
        private void OnCommentViewClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedPost = button?.BindingContext as Post;

            if (selectedPost == null) return;

            // Toggle the visibility of the comments list inside the card
            selectedPost.IsCommentsVisible = !selectedPost.IsCommentsVisible;

            // Handle the Shared Input Bar at the bottom
            if (selectedPost.IsCommentsVisible)
            {
                _activePostForComment = selectedPost;
                IsInputVisible = true; // Show the input bar
                EntryCommentText.Focus(); // Focus the cursor
            }
            else
            {
                _activePostForComment = null;
                IsInputVisible = false; // Hide the input bar
                EntryCommentText.Unfocus();
            }
        }

        // --- SEND COMMENT LOGIC ---
        private void OnCommentSendClicked(object sender, EventArgs e)
        {
            string text = EntryCommentText.Text;

            if (string.IsNullOrWhiteSpace(text) || _activePostForComment == null) return;

            // Add the new comment to the active post
            _activePostForComment.CommentsList.Add(new PostComment
            {
                Username = "Me", // You can replace this with the logged-in user's name
                Text = text
            });

            // Update the counters
            _activePostForComment.Comments++;

            // Clear the input
            EntryCommentText.Text = string.Empty;
        }

        // --- LIKE LOGIC ---
        private void OnLikeClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Post post)
            {
                post.Likes++;
            }
        }

        // --- NAVIGATION LOGIC ---
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;

            // Prevent navigating to the page we are already on
            if (destination == "Community") return;

            if (destination == "List") await Navigation.PushAsync(new AssessmentPage());
            else if (destination == "Diary") await Navigation.PushAsync(new WriteDiaryPage());
            else if (destination == "Stats") await Navigation.PushAsync(new AnalyticPage());
            else if (destination == "Profile") await Navigation.PushAsync(new ProfilePage());
        }
    }
}