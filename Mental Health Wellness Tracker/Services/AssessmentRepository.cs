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
using Google.Cloud.Firestore;
using Google.Protobuf.WellKnownTypes;

namespace Mental_Health_Wellness_Tracker.Services
{
    public class AssessmentRepository : IAssessmentRepository
    {
        private SQLiteAsyncConnection _database;
        private FirestoreDb _firestoreDb;

        // Your Database Project ID
        private const string ProjectId = "mental-health-wellness-tracker";

        // Your Firebase Web API Key
        private const string WebApiKey = "AIzaSyARXVMSRY2JvzMJue2jWUoCd44bv1TYBaE";

        public AssessmentRepository()
        {
            // Leave the constructor empty for now; place the initialization logic in InitAsync.
        }

        //private async Task InitAsync()
        //{
        //    if (_database != null)
        //        return;

        //    // Initialize the SQLite connection
        //    var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MentalHealth.db3");
        //    _database = new SQLiteAsyncConnection(dbPath);

        //    // Create tables if they don't exist
        //    // Create AssessmentResult table for storing user assessment results
        //    await _database.CreateTableAsync<AssessmentResult>();
        //    // Create AssessmentQuestion table for question bank
        //    await _database.CreateTableAsync<AssessmentQuestion>();
        //    // Create DiaryEntry table for diary entries
        //    await _database.CreateTableAsync<DiaryEntry>();
        //    // Create UserProfile table for user profiles
        //    await _database.CreateTableAsync<UserProfile>();
        //    // Create LocalDiaryEntry table for local diary storage
        //    await _database.CreateTableAsync<LocalDiaryEntry>();

        //    // Data Seeding: If the question bank is empty, we automatically fill it with default questions.
        //    if (await _database.Table<AssessmentQuestion>().CountAsync() == 0)
        //    {
        //        await SeedQuestionsAsync();
        //    }

        //    // Initialize Firestore SDK
        //    try
        //    {
        //        _firestoreDb = FirestoreDb.Create(ProjectId);
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Firestore Initialization Warning: {ex.Message}");
        //    }
        //}

        private async Task InitAsync()
        {
            // 1. Initialize connection: Only create a connection if it is empty
            if (_database == null)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MentalHealth.db3");
                _database = new SQLiteAsyncConnection(dbPath);
            }

            // The current logic is that the following code will run every time InitAsync is called,
            // regardless of whether a database connection already exists.
            // SQLite will automatically check and create the table if it doesn't exist.
            // If it already exists, it will be skipped. This is safe.
            // Create tables if they don't exist
            // Create AssessmentResult table for storing user assessment results
            await _database.CreateTableAsync<AssessmentResult>();
            // Create AssessmentQuestion table for question bank
            await _database.CreateTableAsync<AssessmentQuestion>();
            // Create DiaryEntry table for diary entries
            await _database.CreateTableAsync<DiaryEntry>();
            // Create UserProfile table for user profiles
            await _database.CreateTableAsync<UserProfile>();
            // Create LocalDiaryEntry table for local diary storage
            await _database.CreateTableAsync<LocalDiaryEntry>();

            // 2. Data Seed: Fill with the default question(if the question bank is empty)
            if (await _database.Table<AssessmentQuestion>().CountAsync() == 0)
            {
                await SeedQuestionsAsync();
            }

            // 3. Initialize the Firestore SDK(remain unchanged)
            try
            {
                if (_firestoreDb == null) // Add a simple check to prevent duplicate creation
                {
                    _firestoreDb = FirestoreDb.Create(ProjectId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firestore Initialization Warning: {ex.Message}");
            }
        }

        // Diary core functions
        // Core functionality: Add diary entries (offline preferred + REST API upload)
        public async Task AddDiaryEntryAsync(LocalDiaryEntry localEntry)
        {
            await InitAsync();

            // Step 1: First, save to local SQLite (offline protection)
            // Whether you have internet access or not, save it first to ensure your data isn't lost
            localEntry.IsSynced = false;
            await _database.InsertAsync(localEntry);

            // Step 2： Check the network
            // If there is no internet connection or no token, this is the end (local storage only)
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return;

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return;

            try
            {
                // Image upload logic
                // If there is a local image path, and the cloud link has not yet been generated
                if (!string.IsNullOrEmpty(localEntry.LocalImagePath) && string.IsNullOrEmpty(localEntry.ImgUrl))
                {
                    if (File.Exists(localEntry.LocalImagePath))
                    {
                        // Read file stream
                        using var stream = File.OpenRead(localEntry.LocalImagePath);
                        var fileName = $"{Guid.NewGuid()}.jpg"; // Generate unique filenames

                        // Call Storage Service
                        var storageService = new FirebaseStorageService();
                        var downloadUrl = await storageService.UploadImageAsync(stream, fileName, token);

                        if (!string.IsNullOrEmpty(downloadUrl))
                        {
                            // Upload successful, cloud link obtained
                            localEntry.ImgUrl = downloadUrl;
                            // Update the local database (save the cloud link into it)
                            await _database.UpdateAsync(localEntry);
                        }
                    }
                }
                // Step 3: Prepare the REST API request
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // The URL for creating a document using the Firestore REST API
                string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries?key={WebApiKey}";

                // Step 4: Construct the payload
                // The Firestore REST API requires a specific JSON format: { "fields": { "key": { "type": "value" } } }
                var firestorePayload = new
                {
                    fields = new
                    {
                        // String types use stringValue
                        userId = new { stringValue = localEntry.UserId },
                        userEmail = new { stringValue = localEntry.UserEmail ?? "" }, // 防止 null
                        username = new { stringValue = localEntry.Username ?? "Anonymous" },
                        content = new { stringValue = localEntry.Content },

                        // Mood data
                        moodName = new { stringValue = localEntry.MoodName },
                        moodEmoji = new { stringValue = localEntry.MoodEmoji },

                        // Note: Integers must be converted to strings before being passed to integerValue in the Firestore REST API
                        moodScore = new { integerValue = localEntry.MoodScore.ToString() },

                        // This will now send the actual cloud link (if the upload was successful)
                        imgUrl = new { stringValue = localEntry.ImgUrl ?? "" },

                        // Timestamps use timestampValue (ISO 8601 format)
                        dateCreated = new { timestampValue = localEntry.DateCreated.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
                    }
                };

                // Serializing JSON
                var jsonContent = JsonSerializer.Serialize(firestorePayload);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Step 5: Send a POST request to upload to the cloud
                var response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    // Step 6: Upload successful, update local status

                    // Parse the response to obtain the ID generated by Firestore
                    // The response contains "name": "projects/.../documents/diary_entries/documentID"
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("name", out var nameElement))
                    {
                        var path = nameElement.GetString();
                        // Extract the last part as the ID
                        var firestoreId = path.Split('/').Last();
                        localEntry.FirestoreId = firestoreId;
                    }

                    localEntry.IsSynced = true;
                    await _database.UpdateAsync(localEntry);

                    Debug.WriteLine($"Diary synced successfully! ID: {localEntry.FirestoreId}");
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

        public async Task<List<LocalDiaryEntry>> GetLocalDiaryEntriesAsync(string userId)
        {
            await InitAsync();
            return await _database.Table<LocalDiaryEntry>()
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.DateCreated)
                .ToListAsync();
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
                            userEmail = new { stringValue = item.UserEmail ?? "" },
                            username = new { stringValue = item.Username ?? "Anonymous" },
                            q_answer = new { stringValue = item.AnswersJson?? "[]" },
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
                    Debug.WriteLine($"Assessment Sync Exception: {ex.Message}");
                }
            }
        }

        // Download history from the cloud
        public async Task SyncAssessmentFromCloudAsync(string userId)
        {
            // 1. Check network and token
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. Request Firestore to retrieve all documents from the assessments collection
            string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/assessments?key={WebApiKey}";

            try
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Download Error: {response.StatusCode}");
                    return;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                // 3. Parse the returned list of documents
                if (doc.RootElement.TryGetProperty("documents", out JsonElement documents))
                {
                    await InitAsync(); // Ensure the local database is ready

                    foreach (var docElement in documents.EnumerateArray())
                    {
                        // 3.1 Obtain the document ID (FirestoreId)
                        // path 格式: "projects/.../databases/(default)/documents/assessments/DOCUMENT_ID"
                        string path = docElement.GetProperty("name").GetString();
                        string firestoreId = path.Split('/').Last();

                        // 3.2 Parsing Fields
                        var fields = docElement.GetProperty("fields");

                        // Helper function: Securely read Firestore string fields
                        string GetStr(string key) =>
                            fields.TryGetProperty(key, out var f) && f.TryGetProperty("stringValue", out var v) ? v.GetString() : "";

                        // Helper function: Safely read Firestore integer fields
                        int GetInt(string key) =>
                            fields.TryGetProperty(key, out var f) && f.TryGetProperty("integerValue", out var v) && int.TryParse(v.GetString(), out int i) ? i : 0;

                        // Check if this record belongs to the current user
                        string recordUserId = GetStr("userId");
                        if (recordUserId != userId) continue; // This is not my data, skip

                        // 3.3 Check if it already exists locally (to prevent duplicate additions)
                        var existing = await _database.Table<AssessmentResult>()
                                                      .Where(x => x.FirestoreId == firestoreId)
                                                      .FirstOrDefaultAsync();
                        if (existing != null) continue; // If it's already available locally, skip this step

                        // 3.4 Resolution Date
                        DateTime dateTaken = DateTime.Now;
                        if (fields.TryGetProperty("dateTaken", out var dtField) && dtField.TryGetProperty("timestampValue", out var ts))
                        {
                            DateTime.TryParse(ts.GetString(), out dateTaken);
                        }

                        // 3.5 Create and save a local object
                        var newLocalResult = new AssessmentResult
                        {
                            FirestoreId = firestoreId,
                            UserId = recordUserId,
                            UserEmail = GetStr("userEmail"),
                            Username = GetStr("username"),
                            TestType = GetStr("testType"),
                            TotalScore = GetInt("totalScore"),
                            CalculatedResult = GetStr("calculatedResult"),
                            AnswersJson = GetStr("q_answer"), // Save the JSON string directly
                            DateTaken = dateTaken,
                            IsSynced = true // Since it was downloaded from the cloud, it must have been synced
                        };

                        await _database.InsertAsync(newLocalResult);
                        Debug.WriteLine($"Downloaded assessment: {firestoreId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Sync Download Exception: {ex.Message}");
            }
        }

        // Diary Entry Methods
        //public async Task<bool> SaveDiaryEntryAsync(DiaryEntry entry)
        //{
        //    await InitAsync();
        //    try
        //    {
        //        entry.IsSynced = false; // Marked as not synchronized
        //        await _database.InsertAsync(entry);

        //        // Try background synchronization
        //        _ = SyncPendingDiaryEntriesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine($"Diary Save Error: {ex.Message}");
        //        return false;
        //    }
        //}

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
        //public async Task SyncPendingDiaryEntriesAsync()
        //{
        //    await InitAsync();

        //    if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        //        return; // No internet access, exit early

        //    // Retrieve all diary entries where IsSynced = false
        //    // Get all unsynced diary entries
        //    var unsyncedDiaries = await _database.Table<DiaryEntry>()
        //        .Where(x => x.IsSynced == false)
        //        .ToListAsync();

        //    if (unsyncedDiaries.Count == 0)
        //        return; // No items to sync

        //    var token = await SecureStorage.GetAsync("auth_token");
        //    if (string.IsNullOrEmpty(token))
        //        return; // No auth token, cannot sync

        //    using var client = new HttpClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    // Note: Here we will store the diary entries in a new collection 'diary_entries'
        //    string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries?key={WebApiKey}";

        //    foreach (var item in unsyncedDiaries)
        //    {
        //        try
        //        {
        //            var firestorePayload = new
        //            {
        //                fields = new
        //                {
        //                    userId = new { stringValue = item.UserId },
        //                    username = new { stringValue = item.Username ?? "Anonymous" },
        //                    content = new { stringValue = item.Content },
        //                    moodEmoji = new { stringValue = item.MoodEmoji },
        //                    dateCreated = new { timestampValue = item.DateCreated.ToString("yyyy-MM-ddTHH:mm:ssZ") }
        //                }
        //            };

        //            var jsonContent = JsonSerializer.Serialize(firestorePayload);
        //            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

        //            var response = await client.PostAsync(url, content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                item.IsSynced = true;
        //                await _database.UpdateAsync(item);
        //                Debug.WriteLine($"Synced Diary {item.Id} successfully!");
        //            }
        //            else
        //            {
        //                var error = await response.Content.ReadAsStringAsync();
        //                Debug.WriteLine($"Diary Sync Failed: {error}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine($"Diary Sync Exception: {ex.Message}");
        //        }
        //    }
        //}

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
