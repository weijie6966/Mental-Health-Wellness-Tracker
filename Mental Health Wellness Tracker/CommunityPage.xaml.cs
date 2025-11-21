using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;
using Microsoft.Maui.Storage;

namespace Mental_Health_Wellness_Tracker
{
    // 1. COMMENT MODEL
    public class PostComment
    {
        public string Username { get; set; }
        public string Text { get; set; }
    }

    // 2. POST MODEL
    public class Post : INotifyPropertyChanged
    {
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MoodEmoji { get; set; } = "emoji_neutral.png";

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
        private Post _activePostForComment;
        private bool _isInputVisible;

        // Database repository
        private readonly IAssessmentRepository _repository;

        public bool IsInputVisible
        {
            get => _isInputVisible;
            set
            {
                _isInputVisible = value;
                OnPropertyChanged(nameof(IsInputVisible));
            }
        }

        public CommunityPage(IAssessmentRepository repository)
        {
            InitializeComponent();
            BindingContext = this;

            // Assigning values ​​to fields
            _repository = repository;

            // Initialize collection
            Posts = new ObservableCollection<Post>();
            PostsCollection.ItemsSource = Posts;
        }

        // Reload data whenever the page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPosts();
        }

        private async System.Threading.Tasks.Task LoadPosts()
        {
            if (_repository == null) 
                return; // Security check

            Posts.Clear();

            var cloudDiaries = await _repository.GetAllDiaryEntriesFromCloudAsync();

            if (cloudDiaries.Count > 0)
            {
                // If internet connection is available and diaries are retrieved from the cloud, use them.
                foreach (var diary in cloudDiaries)
                {
                    Posts.Add(new Post
                    {
                        Username = string.IsNullOrEmpty(diary.Username) ? "Unknown" : diary.Username,
                        Content = diary.Content,
                        MoodEmoji = diary.MoodEmoji,
                        Likes = 0,
                        Comments = 0
                    });
                }
            }
            else
            {
                // If there is no internet connection or the retrieval fails, load your local logs (fallback solution).
                string userId = await SecureStorage.GetAsync("user_id") ?? "unknown_user";
                string myName = Preferences.Get("UsernameKey", "Me");
                var localDiaries = await _repository.GetDiaryEntriesAsync(userId);

                foreach (var diary in localDiaries)
                {
                    Posts.Add(new Post { Username = myName, Content = diary.Content, MoodEmoji = diary.MoodEmoji });
                }
            }

            // Add dummy data
            Posts.Add(new Post
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
            });

            Posts.Add(new Post
            {
                Username = "Mike22",
                Content = "Anxiety is high today. Breathing exercises aren't working.",
                MoodEmoji = "emoji_sad.png",
                Likes = 2,
                Comments = 0
            });
        }

        // --- UI Interaction Logic ---
        private void OnCommentViewClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var selectedPost = button?.BindingContext as Post;
            if (selectedPost == null) return;

            selectedPost.IsCommentsVisible = !selectedPost.IsCommentsVisible;

            if (selectedPost.IsCommentsVisible)
            {
                _activePostForComment = selectedPost;
                IsInputVisible = true;
                EntryCommentText.Focus();
            }
            else
            {
                _activePostForComment = null;
                IsInputVisible = false;
                EntryCommentText.Unfocus();
            }
        }

        private void OnCommentSendClicked(object sender, EventArgs e)
        {
            string text = EntryCommentText.Text;
            if (string.IsNullOrWhiteSpace(text) || _activePostForComment == null) return;

            _activePostForComment.CommentsList.Add(new PostComment
            {
                Username = "Me",
                Text = text
            });
            _activePostForComment.Comments++;
            EntryCommentText.Text = string.Empty;
        }

        private void OnLikeClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Post post)
            {
                post.Likes++;
            }
        }

        // Navigation logic
        private async void OnNavTapped(object sender, EventArgs e)
        {
            string destination = ((Button)sender).AutomationId;
            if (destination == "Community") return;

            // Retrieve the currently held repo and pass it to other pages.
            if (destination == "List")
            {
                await Navigation.PushAsync(new AssessmentPage(_repository));
            }
            else if (destination == "Diary")
            {
                // WriteDiaryPage needs to be obtained using ServiceLocator because it doesn't yet have constructor injection (or we can modify it).
                // Let's keep it simple and let WriteDiaryPage handle it itself.
                await Navigation.PushAsync(new WriteDiaryPage());
            }
            else if (destination == "Stats")
            {
                await Navigation.PushAsync(new AnalyticPage(_repository));
            }
            else if (destination == "Profile")
            {
                await Navigation.PushAsync(new ProfilePage());
            }
        }
    }
}