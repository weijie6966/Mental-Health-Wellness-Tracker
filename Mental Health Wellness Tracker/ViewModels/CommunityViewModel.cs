using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class CommunityViewModel : ViewModelBase
    {
        // Static collection remains here for global access (shared state)
        public static ObservableCollection<Post> SharedPosts { get; set; } = new ObservableCollection<Post>();

        // Public property to bind the CollectionView to
        public ObservableCollection<Post> Posts => SharedPosts;

        private Post _activePostForComment;
        private string _commentText;

        // Properties for the comment input field
        public string CommentText
        {
            get => _commentText;
            set { _commentText = value; OnPropertyChanged(); ((RelayCommand)CommentSendCommand).RaiseCanExecuteChanged(); }
        }

        private bool _isInputVisible;
        public bool IsInputVisible
        {
            get => _isInputVisible;
            set { _isInputVisible = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand DeleteCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand CommentViewToggleCommand { get; }
        public ICommand CommentSendCommand { get; }
        public ICommand LikeCommand { get; }
        public ICommand NavigateCommand { get; }

        public CommunityViewModel()
        {
            // Initialize Commands
            DeleteCommand = new RelayCommand(async param => await OnDeleteClicked(param as Post));
            EditCommand = new RelayCommand(async param => await OnEditClicked(param as Post));
            CommentViewToggleCommand = new RelayCommand(OnCommentViewToggleClicked);
            CommentSendCommand = new RelayCommand(OnCommentSendClicked, CanSendComment);
            LikeCommand = new RelayCommand(OnLikeClicked);
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // Helper to check if a comment can be sent
        private bool CanSendComment(object parameter)
        {
            return _activePostForComment != null && !string.IsNullOrWhiteSpace(CommentText);
        }

        // --- Core Logic (Moved from CommunityPage.xaml.cs) ---

        private async Task OnDeleteClicked(Post postToDelete)
        {
            if (postToDelete == null) return;

            bool answer = await Application.Current.MainPage.DisplayAlert("Delete Post", "Are you sure?", "Yes", "No");
            if (answer)
            {
                SharedPosts.Remove(postToDelete);
            }
        }

        private async Task OnEditClicked(Post postToEdit)
        {
            if (postToEdit == null) return;

            string result = await Application.Current.MainPage.DisplayPromptAsync("Edit Post", "Update text:", initialValue: postToEdit.Content);
            if (result != null)
            {
                postToEdit.Content = result;
            }
        }

        private void OnCommentViewToggleClicked(object parameter)
        {
            var selectedPost = parameter as Post;
            if (selectedPost == null) return;

            selectedPost.IsCommentsVisible = !selectedPost.IsCommentsVisible;

            if (selectedPost.IsCommentsVisible)
            {
                _activePostForComment = selectedPost;
                IsInputVisible = true;
            }
            else
            {
                _activePostForComment = null;
                IsInputVisible = false;
                CommentText = string.Empty; // Clear input when closing
            }
        }

        private void OnCommentSendClicked(object parameter)
        {
            if (_activePostForComment == null || string.IsNullOrWhiteSpace(CommentText)) return;

            _activePostForComment.CommentsList.Add(new PostComment
            {
                Username = Preferences.Get("UsernameKey", "Me"),
                Text = CommentText,
                CommentTime = DateTime.Now
            });

            // Manually trigger property change to update the displayed comment count
            _activePostForComment.Comments++;

            // Clear the input text (triggers CanExecuteChanged)
            CommentText = string.Empty;
        }

        private void OnLikeClicked(object parameter)
        {
            if (parameter is Post p)
            {
                p.Likes++;
            }
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Community") return;

            Page nextPage = destination switch
            {
                "List" => new AssessmentPage(),
                "Diary" => new WriteDiaryPage(),
                "Stats" => new AnalyticPage(),
                "Profile" => new ProfilePage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}