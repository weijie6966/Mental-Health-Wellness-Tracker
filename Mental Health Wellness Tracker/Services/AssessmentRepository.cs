using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Mental_Health_Wellness_Tracker.Models;
using System.IO;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;

namespace Mental_Health_Wellness_Tracker.Services
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private SQLiteAsyncConnection _database;

        // Your Database Project ID
        private const string ProjectId = "mental-health-wellness-tracker";

        // Your Firebase Web API Key
        private const string WebApiKey = "AIzaSyARXVMSRY2JvzMJue2jWUoCd44bv1TYBaE";

        public AssessmentRepository()
        {
            // Leave the constructor empty for now; place the initialization logic in InitAsync.
        }

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            // Initialize the SQLite connection
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MentalHealth.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            // Create tables if they don't exist
            // Create AssessmentResult table for storing user assessment results
            await _database.CreateTableAsync<AssessmentResult>();
            // Create AssessmentQuestion table for question bank
            await _database.CreateTableAsync<AssessmentQuestion>();
            // Create DiaryEntry table for diary entries
            await _database.CreateTableAsync<DiaryEntry>();
            // Create UserProfile table for user profiles
            await _database.CreateTableAsync<UserProfile>();

            // Data Seeding: If the question bank is empty, we automatically fill it with default questions.
            if (await _database.Table<AssessmentQuestion>().CountAsync() == 0)
            {
                await SeedQuestionsAsync();
            }
        }

        // Get the question
        public async Task<List<AssessmentQuestion>> GetQuestionsByTestTypeAsync(string testType)
        {
            await InitAsync();
            return await _database.Table<AssessmentQuestion>()
                .Where(q => q.TestType == testType)
                .OrderBy(q => q.OrderIndex)
                .ToListAsync();
        }

        // Save the assessment result
        public async Task<bool> SaveAssessmentResultAsync(AssessmentResult result)
        {
            await InitAsync();
            try
            {
                // New assessment results are marked as not synced by default
                // The default setting is not synchronized; it will wait for subsequent network uploads.
                result.IsSynced = false;
                await _database.InsertAsync(result);
                // Try syncing immediately (Fire and forget, without blocking the UI).
                // We don't wait for it to finish. We let it run in the background.
                _ = SyncPendingAssessmentsAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return false;
            }
        }

        // Get assessment history for a user
        public async Task<List<AssessmentResult>> GetAssessmentHistoryAsync(string userId)
        {
            await InitAsync();
            return await _database.Table<AssessmentResult>()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateTaken)
                .ToListAsync();
        }

        // Sync pending assessments to remote server
        // Core: Synchronization functionality implementation (REST API)
        public async Task SyncPendingAssessmentsAsync()
        {
            await InitAsync();

            // Check network connectivity
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return; // No internet access, exit early

            // Retrieve all records where IsSynced = false
            var unsyncedItems = await _database.Table<AssessmentResult>()
                .Where(x => x.IsSynced == false)
                .ToListAsync();

            if (unsyncedItems.Count == 0)
                return; // No items to sync

            // Obtain the Auth Token (a token is required to write to Firestore).
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return; // No auth token, cannot sync

            using var client = new HttpClient();
            // Set the Authorization header with the Bearer token
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Firestore REST API endpoint for adding documents
            string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/assessments?key={WebApiKey}";

            foreach (var item in unsyncedItems)
            {
                try
                {
                    // Build Firestore-specific JSON format
                    // Firestore requires this format: { "fields": { "key": { "stringValue": "val" } } }
                    var firestorePayload = new
                    {
                        fields = new
                        {
                            userId = new { stringValue = item.UserId },
                            testType = new { stringValue = item.TestType },
                            totalScore = new { integerValue = item.TotalScore.ToString() }, // Firestore expects integerValue as string
                            calculatedResult = new { stringValue = item.CalculatedResult },
                            dateTaken = new { timestampValue = item.DateTaken.ToString("yyyy-MM-ddTHH:mm:ssZ") }
                        }
                    };

                    var jsonContent = JsonSerializer.Serialize(firestorePayload);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    // Make the POST request
                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Success! Local status updated to synchronized.
                        item.IsSynced = true;
                        // Also, save the ID generated by Firestore (it's in the response, but we'll simplify it here and not parse it).
                        await _database.UpdateAsync(item);
                        Debug.WriteLine($"Synced item {item.LocalId} successfully!");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Sync Failed: {error}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Sync Exception: {ex.Message}");
                }
            }
        }

        // Diary Entry Methods
        public async Task<bool> SaveDiaryEntryAsync(DiaryEntry entry)
        {
            await InitAsync();
            try
            {
                entry.IsSynced = false; // Marked as not synchronized
                await _database.InsertAsync(entry);

                // Try background synchronization
                _ = SyncPendingDiaryEntriesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Diary Save Error: {ex.Message}");
                return false;
            }
        }

        // Sync pending diary entries to remote server
        // Get Diary List
        public async Task<List<DiaryEntry>> GetDiaryEntriesAsync(string userId)
        {
            await InitAsync();
            return await _database.Table<DiaryEntry>()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.DateCreated) // Reverse chronological order
                .ToListAsync();
        }

        // Sync pending diary entries to remote server
        // Sync logs to Firebase Firestore
        public async Task SyncPendingDiaryEntriesAsync()
        {
            await InitAsync();

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return; // No internet access, exit early

            // Retrieve all diary entries where IsSynced = false
            // Get all unsynced diary entries
            var unsyncedDiaries = await _database.Table<DiaryEntry>()
                .Where(x => x.IsSynced == false)
                .ToListAsync();

            if (unsyncedDiaries.Count == 0)
                return; // No items to sync

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return; // No auth token, cannot sync

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Note: Here we will store the diary entries in a new collection 'diary_entries'
            string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries?key={WebApiKey}";

            foreach (var item in unsyncedDiaries)
            {
                try
                {
                    var firestorePayload = new
                    {
                        fields = new
                        {
                            userId = new { stringValue = item.UserId },
                            username = new { stringValue = item.Username ?? "Anonymous" },
                            content = new { stringValue = item.Content },
                            moodEmoji = new { stringValue = item.MoodEmoji },
                            dateCreated = new { timestampValue = item.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ") }
                        }
                    };

                    var jsonContent = JsonSerializer.Serialize(firestorePayload);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        item.IsSynced = true;
                        await _database.UpdateAsync(item);
                        Debug.WriteLine($"Synced Diary {item.Id} successfully!");
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine($"Diary Sync Failed: {error}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Diary Sync Exception: {ex.Message}");
                }
            }
        }

        // Retrieve everyone's diaries from the cloud (for the community page).
        public async Task<List<DiaryEntry>> GetAllDiaryEntriesFromCloudAsync()
        {
            // Check the network
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new List<DiaryEntry>(); // No internet access, return empty list

            // Get Auth Token
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return new List<DiaryEntry>();

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // No auth token needed for public read access
            string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries?key={WebApiKey}";

            try
            {
                // Make the GET request
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var resultList = new List<DiaryEntry>();

                    // Parse the JSON response
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        if (doc.RootElement.TryGetProperty("documents", out JsonElement documents))
                        {
                            foreach (var docElement in documents.EnumerateArray())
                            {
                                var fields = docElement.GetProperty("fields");

                                // Helper function to extract string values safely
                                string GetString(JsonElement elem, string key) =>
                                    elem.TryGetProperty(key, out var child) && child.TryGetProperty("stringValue", out var val) ? val.GetString() : "";

                                string dateStr = GetString(fields, "dateCreated"); // 可能是 timestampValue
                                if (fields.TryGetProperty("dateCreated", out var dateField) && dateField.TryGetProperty("timestampValue", out var ts))
                                {
                                    dateStr = ts.GetString();
                                }

                                resultList.Add(new DiaryEntry
                                {
                                    UserId = GetString(fields, "userId"),
                                    Username = GetString(fields, "username"), // Get Username
                                    Content = GetString(fields, "content"),
                                    MoodEmoji = GetString(fields, "moodEmoji"),
                                    DateCreated = DateTime.TryParse(dateStr, out var dt) ? dt : DateTime.Now,
                                    IsSynced = true // The data from the cloud has definitely been synchronized.
                                });
                            }
                        }
                    }

                    return resultList.OrderByDescending(d => d.DateCreated).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fetch Cloud Error: {ex.Message}");
            }

            return new List<DiaryEntry>();
        }

        public async Task<bool> SaveUserProfileAsync(UserProfile profile)
        {
            await InitAsync();
            try
            {
                // Check if the user already has a profile.
                var existingProfile = await _database.Table<UserProfile>()
                                                     .Where(p => p.UserId == profile.UserId)
                                                     .FirstOrDefaultAsync();

                if (existingProfile != null)
                {
                    // If so, update it (keeping the ID unchanged).
                    profile.Id = existingProfile.Id;
                    await _database.UpdateAsync(profile);
                }
                else
                {
                    // If not, create a new one.
                    await _database.InsertAsync(profile);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Profile Save Error: {ex.Message}");
                return false;
            }
        }

        public async Task<UserProfile> GetUserProfileAsync(string userId)
        {
            await InitAsync();
            // Find the file belonging to this UserID.
            return await _database.Table<UserProfile>()
                                  .Where(p => p.UserId == userId)
                                  .FirstOrDefaultAsync();
        }

        // Seed default questions into the database
        private async Task SeedQuestionsAsync()
        {
            var questions = new List<AssessmentQuestion>();

            // Rosenberg Self-Esteem Scale Questions
            // Note: Questions 3, 5, 8, 9, and 10 are usually scored in reverse (IsReversed = true).
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 1, QuestionText = "I feel that I am a person of worth, at least on an equal plane with others.", IsReversed = false, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 2, QuestionText = "I feel that I have a number of good qualities.", IsReversed = false, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 3, QuestionText = "All in all, I am inclined to feel that I am a failure.", IsReversed = true, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 4, QuestionText = "I am able to do things as well as most other people.", IsReversed = false, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 5, QuestionText = "I feel I do not have much to be proud of.", IsReversed = true, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 6, QuestionText = "I take a positive attitude toward myself.", IsReversed = false, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 7, QuestionText = "On the whole, I am satisfied with myself.", IsReversed = false, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 8, QuestionText = "I wish I could have more respect for myself.", IsReversed = true, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 9, QuestionText = "I certainly feel useless at times.", IsReversed = true, MaxScore = 3 });
            questions.Add(new AssessmentQuestion { TestType = "Rosenberg", OrderIndex = 10, QuestionText = "At times I think I am no good at all.", IsReversed = true, MaxScore = 3 });

            // Perceived Stress Scale Questions
            // Note: Questions 4, 5, 7, and 8 are usually scored in reverse.
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 1, QuestionText = "In the last month, how often have you been upset because of something that happened unexpectedly?", IsReversed = false, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 2, QuestionText = "In the last month, how often have you felt that you were unable to control the important things in your life?", IsReversed = false, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 4, QuestionText = "In the last month, how often have you felt confident about your ability to handle your personal problems?", IsReversed = true, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 5, QuestionText = "In the last month, how often have you felt that things were going your way?", IsReversed = true, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 6, QuestionText = "In the last month, how often have you found that you could not cope with all the things that you had to do?", IsReversed = false, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 7, QuestionText = "In the last month, how often have you been able to control irritations in your life?", IsReversed = true, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 8, QuestionText = "In the last month, how often have you felt that you were on top of things?", IsReversed = true, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 9, QuestionText = "In the last month, how often have you been angered because of things that were outside of your control?", IsReversed = false, MaxScore = 4 });
            questions.Add(new AssessmentQuestion { TestType = "PSS", OrderIndex = 10, QuestionText = "In the last month, how often have you felt difficulties were piling up so high that you could not overcome them?", IsReversed = false, MaxScore = 4 });

            // Batch insert into database
            await _database.InsertAllAsync(questions);
        }
    }
}
