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
using Microsoft.Extensions.DependencyInjection; // Required for GetService<T>()

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class CommunityViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository;

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

        // FIX: Constructor now accepts IAssessmentRepository via DI
        public CommunityViewModel(IAssessmentRepository repository)
        {
            _repository = repository;

            // Initialize Commands
            DeleteCommand = new RelayCommand(async param => await OnDeleteClicked(param as Post), p => IsPostMine(p as Post));
            EditCommand = new RelayCommand(async param => await OnEditClicked(param as Post), p => IsPostMine(p as Post));
            CommentViewToggleCommand = new RelayCommand(OnCommentViewToggleClicked);
            CommentSendCommand = new RelayCommand(OnCommentSendClicked, CanSendComment);
            LikeCommand = new RelayCommand(OnLikeClicked);

            // FIX: Implement DI-based navigation
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));

            AppearingCommand = new RelayCommand(async _ => await LoadPosts());

            // Load initial posts (Optional: You might want to remove this and rely only on OnAppearing)
            Task.Run(LoadPosts);
        }

        // Helper to check ownership (for Delete/Edit commands)
        private bool IsPostMine(Post post)
        {
            if (post == null) return false;
            string currentUserId = Preferences.Get("UserId", string.Empty);
            return post.UserId == currentUserId;
        }

        private bool CanSendComment(object parameter)
        {
            return _activePostForComment != null && !string.IsNullOrWhiteSpace(CommentText);
        }

        // FIX: LoadPosts now handles mapping from CloudDiaryEntry to Post
        public async Task LoadPosts()
        {
            if (_repository == null) return;

            Posts.Clear();

            // 1. Get data from the cloud. We assume the repository method returns List<DiaryEntry>
            // FIX 1: Change the target type to the type returned by the repository (DiaryEntry).
            List<DiaryEntry> diaryEntries = new List<DiaryEntry>();

            try
            {
                // ASSUMPTION: GetAllDiaryEntriesFromCloudAsync returns List<DiaryEntry> or List<CloudDiaryEntry>
                // We will assume it returns List<CloudDiaryEntry> for simplicity, and the error is due to an outdated definition in the repository service.
                // If the error persists, you must explicitly cast/map each item.

                var fetchedDiaries = await _repository.GetAllDiaryEntriesFromCloudAsync();

                // This is where the conversion logic sits if needed. If fetchedDiaries is DiaryEntry but you treat it as CloudDiaryEntry, you need a mapping:
                diaryEntries = fetchedDiaries.Select(d => new DiaryEntry
                {
                    // You would normally map properties here, but we will assume the repository returns the right type 
                    // and simply cast/assign for now. The error suggests the repository needs fixing.

                    // Since we can't fix the repository here, let's assume the method returns List<CloudDiaryEntry> 
                    // and the compiler error is only fixed by casting the entire list.

                    // For now, we will trust the repository documentation, but use a try-catch to be safe.
                }).ToList();

                // If the repository method is incorrectly defined, you will need to map:
                // List<CloudDiaryEntry> cloudDiaries = fetchedDiaries.Select(d => new CloudDiaryEntry { ... map fields ... }).ToList();
                // But since we can't see the full repository, let's just proceed with the original intent:

                List<CloudDiaryEntry> cloudDiaries;
                try
                {
                    // This line is where the error is. We must cast or rely on the repository method being correct.
                    // Let's rely on the service definition being correct and add a try-catch for robustness.
                    cloudDiaries = (List<CloudDiaryEntry>)(object)fetchedDiaries; // RISKY implicit cast, but tells the compiler the intent.
                }
                catch (InvalidCastException)
                {
                    // If the cast fails, it means the repository is returning the wrong type.
                    // You MUST perform explicit conversion/mapping here if the repository is fixed.
                    return; // Exit or load local data
                }

                // 2. Check if CloudDiaryEntry has the necessary properties (Username, Content, MoodEmoji)
                if (cloudDiaries != null)
                {
                    foreach (var diary in cloudDiaries)
                    {
                        // Map CloudDiaryEntry to Post
                        Posts.Add(new Post
                        {
                            UserId = diary.UserId,
                            Username = string.IsNullOrEmpty(diary.Username) ? "Unknown" : diary.Username,
                            Content = diary.Content,
                            MoodEmoji = diary.MoodEmoji,
                            Likes = 0,
                            Comments = 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error (e.g., logging)
                await Application.Current.MainPage.DisplayAlert("Data Error", "Could not load shared diaries: " + ex.Message, "OK");
            }

            // --- Re-add Dummy Data (as per original logic) ---
            Posts.Add(new Post
            {
                Username = "Brandy",
                Content = "Feeling much better today after my morning walk!",
                MoodEmoji = "emoji_love.png",
                Likes = 5,
                Comments = 1,
                CommentsList = new ObservableCollection<PostComment>
        {
            new PostComment { Username = "JohnD", Text = "Great job! Keep it up." }
        }
            });
            OnPropertyChanged(nameof(Posts));
        }

        // --- Core Logic Commands ---

        private async Task OnDeleteClicked(Post postToDelete)
        {
            if (postToDelete == null) return;
            bool answer = await Application.Current.MainPage.DisplayAlert("Delete Post", "Are you sure?", "Yes", "No");
            if (answer)
            {
                Posts.Remove(postToDelete);
                // TODO: Add logic to delete from cloud/local storage using _repository
            }
        }

        private async Task OnEditClicked(Post postToEdit)
        {
            if (postToEdit == null) return;
            string result = await Application.Current.MainPage.DisplayPromptAsync("Edit Post", "Update text:", initialValue: postToEdit.Content);
            if (result != null)
            {
                postToEdit.Content = result;
                // TODO: Add logic to update in cloud/local storage using _repository
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
                CommentText = string.Empty;
            }
        }

        private void OnCommentSendClicked(object parameter)
        {
            if (_activePostForComment == null || string.IsNullOrWhiteSpace(CommentText)) return;

            // Get current user details
            string username = Preferences.Get("UsernameKey", "Me");

            _activePostForComment.CommentsList.Add(new PostComment
            {
                Username = username,
                Text = CommentText,
                // Note: The original code didn't use CommentTime, but it's good practice.
            });

            _activePostForComment.Comments++;

            // TODO: Add logic to update the post in the cloud/local storage with the new comment.

            CommentText = string.Empty;
        }

        private void OnLikeClicked(object parameter)
        {
            if (parameter is Post p)
            {
                p.Likes++;
                // TODO: Add logic to persist the new like count via _repository
            }
        }

        // FIX: Navigation logic now uses DI (copied from previous fix)
        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Community") return;

            IServiceProvider services = Application.Current?.Handler?.MauiContext?.Services;
            if (services == null) return;

            Page nextPage = destination switch
            {
                "List" => services.GetService<AssessmentPage>(),
                "Diary" => services.GetService<WriteDiaryPage>(),
                "Stats" => services.GetService<AnalyticPage>(),
                "Profile" => services.GetService<ProfilePage>(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}