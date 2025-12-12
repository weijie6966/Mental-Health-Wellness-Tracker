using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mental_Health_Wellness_Tracker.Models;

namespace Mental_Health_Wellness_Tracker.Services
{
    public interface IAssessmentRepository
    {
        // Retrieve all questions for a specific test (e.g., all questions for "PSS").
        Task<List<AssessmentQuestion>> GetQuestionsByTestTypeAsync(string testType);

        // Save user evaluation results
        Task<bool> SaveAssessmentResultAsync(AssessmentResult result);

        // Retrieve assessment history for a specific user
        Task<List<AssessmentResult>> GetAssessmentHistoryAsync(string userId);
        Task<List<string>> GetAvailableTestTypesAsync();
        Task<string> ChooseRandomTestTypeAsync();

        // Sync pending assessments that have not yet been uploaded to the server
        Task SyncPendingAssessmentsAsync();

        // Diary entry methods
        Task AddDiaryEntryAsync(LocalDiaryEntry entry);
        // Retrieve diary entries for a specific user
        Task<List<LocalDiaryEntry>> GetLocalDiaryEntriesAsync(string userId);
        //// Sync pending diary entries that have not yet been uploaded to the server
        //Task SyncPendingDiaryEntriesAsync();
        // Retrieve all diary entries from the cloud (for community page)
        Task<List<CloudDiaryEntry>> GetAllDiaryEntriesFromCloudAsync();
        Task<bool> UpdateDiaryEntryContentAsync(string firestoreId, string content);
        Task<bool> DeleteDiaryEntryAsync(string firestoreId);
        Task<PostComment> AddDiaryCommentAsync(string diaryId, PostComment comment);
        Task<List<PostComment>> GetDiaryCommentsAsync(string diaryId);
        Task<int?> UpdateDiaryLikesAsync(string diaryId, int newLikeCount);
        Task<int?> UpdateDiaryHugsAsync(string diaryId, int newHugCount);
        // User profile methods
        Task<bool> SaveUserProfileAsync(UserProfile profile);
        // Retrieve user profile by user ID
        Task<UserProfile> GetUserProfileAsync(string userId);
        // Sync assessments history from cloud to local database
        Task SyncAssessmentFromCloudAsync(string userId);
    }
}
