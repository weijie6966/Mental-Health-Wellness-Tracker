using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Views; // Required for Page references

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class CommunityViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository = new AssessmentRepository();
        private string _currentUserId;
        private bool _isLoading;

        // Collection to bind to the CollectionView
        public ObservableCollection<Post> Posts { get; } = new ObservableCollection<Post>();

        public void AddNewPost(Post newEntry)
        {
            if (newEntry != null)
            {
                // Add the new post to the bindable collection at the top
                Posts.Insert(0, newEntry);
            }
        }

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
        public ICommand AppearingCommand { get; } // Command to run LoadPosts on page appearing

        public CommunityViewModel()
        {
            // Initialize Commands
            DeleteCommand = new RelayCommand(async param => await OnDeleteClicked(param as Post), p => IsPostMine(p as Post));
            EditCommand = new RelayCommand(async param => await OnEditClicked(param as Post), p => IsPostMine(p as Post));
            CommentViewToggleCommand = new RelayCommand(async param => await OnCommentViewToggleClicked(param));
            CommentSendCommand = new RelayCommand(OnCommentSendClicked, CanSendComment);
            LikeCommand = new RelayCommand(OnLikeClicked);

            // FIX: Implement DI-based navigation
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));

            AppearingCommand = new RelayCommand(async _ => await LoadPosts());

            _ = LoadUserIdAsync();

            // Load initial posts (Optional: You might want to remove this and rely only on OnAppearing)
            Task.Run(LoadPosts);
        }

        // Helper to check ownership (for Delete/Edit commands)
        private bool IsPostMine(Post post)
        {
            if (post == null) return false;
            return !string.IsNullOrEmpty(_currentUserId) && post.UserId == _currentUserId;
        }

        private async Task LoadUserIdAsync()
        {
            _currentUserId = await SecureStorage.GetAsync("user_id");
        }

        private bool CanSendComment(object parameter)
        {
            return _activePostForComment != null && !string.IsNullOrWhiteSpace(CommentText);
        }

        // FIX: LoadPosts now handles mapping from CloudDiaryEntry to Post
        public async Task LoadPosts()
        {
            if (_repository == null || _isLoading) return;

            _isLoading = true;
            Posts.Clear();

            try
            {
                var fetchedDiaries = await _repository.GetAllDiaryEntriesFromCloudAsync();

                foreach (var diary in fetchedDiaries)
                {
                    Posts.Add(new Post
                    {
                        FirestoreId = diary.Id,
                        UserId = diary.UserId,
                        Username = string.IsNullOrEmpty(diary.Username) ? "Unknown" : diary.Username,
                        Content = diary.Content,
                        MoodEmoji = string.IsNullOrEmpty(diary.MoodEmoji) ? "emoji_neutral.png" : diary.MoodEmoji,
                        Likes = diary.Likes,
                        Comments = diary.CommentsCount,
                        CommentsList = new ObservableCollection<PostComment>()
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Data Error", "Could not load shared diaries: " + ex.Message, "OK");
            }
            finally
            {
                _isLoading = false;
            }

            OnPropertyChanged(nameof(Posts));
        }

        // --- Core Logic Commands ---

        private async Task OnDeleteClicked(Post postToDelete)
        {
            if (postToDelete == null) return;
            bool answer = await Application.Current.MainPage.DisplayAlert("Delete Post", "Are you sure?", "Yes", "No");
            if (answer)
            {
                bool deleted = await _repository.DeleteDiaryEntryAsync(postToDelete.FirestoreId);
                if (deleted)
                {
                    Posts.Remove(postToDelete);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Could not delete the post.", "OK");
                }
            }
        }

        private async Task OnEditClicked(Post postToEdit)
        {
            if (postToEdit == null) return;
            string result = await Application.Current.MainPage.DisplayPromptAsync("Edit Post", "Update text:", initialValue: postToEdit.Content);
            if (result != null)
            {
                postToEdit.Content = result;
                bool updated = await _repository.UpdateDiaryEntryContentAsync(postToEdit.FirestoreId, result);
                if (!updated)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Could not update the post.", "OK");
                }
            }
        }

        private async Task OnCommentViewToggleClicked(object parameter)
        {
            var selectedPost = parameter as Post;
            if (selectedPost == null) return;

            selectedPost.IsCommentsVisible = !selectedPost.IsCommentsVisible;

            if (selectedPost.IsCommentsVisible)
            {
                _activePostForComment = selectedPost;
                IsInputVisible = true;
                if (selectedPost.CommentsList.Count == 0)
                {
                    var comments = await _repository.GetDiaryCommentsAsync(selectedPost.FirestoreId);
                    selectedPost.CommentsList.Clear();
                    foreach (var comment in comments)
                    {
                        selectedPost.CommentsList.Add(comment);
                    }
                    selectedPost.Comments = comments.Count;
                }
            }
            else
            {
                _activePostForComment = null;
                IsInputVisible = false;
                CommentText = string.Empty;
            }
        }

        private void OnCommentSendClicked(object parameter)
        {
            _ = SendCommentAsync();
        }

        private async Task SendCommentAsync()
        {
            if (_activePostForComment == null || string.IsNullOrWhiteSpace(CommentText)) return;

            string username = Preferences.Get("UsernameKey", "Me");

            var comment = new PostComment
            {
                Username = username,
                Text = CommentText,
                CommentTime = DateTime.UtcNow
            };

            var saved = await _repository.AddDiaryCommentAsync(_activePostForComment.FirestoreId, comment);
            if (saved != null)
            {
                _activePostForComment.CommentsList.Add(saved);
                _activePostForComment.Comments++;
                CommentText = string.Empty;
                ((RelayCommand)CommentSendCommand).RaiseCanExecuteChanged();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Could not post comment right now.", "OK");
            }
        }

        private void OnLikeClicked(object parameter)
        {
            if (parameter is Post p)
            {
                _ = UpdateLikesAsync(p);
            }
        }

        private async Task UpdateLikesAsync(Post post)
        {
            var newCount = post.Likes + 1;
            var updated = await _repository.UpdateDiaryLikesAsync(post.FirestoreId, newCount);

            if (updated.HasValue)
            {
                post.Likes = updated.Value;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Could not like this post right now.", "OK");
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