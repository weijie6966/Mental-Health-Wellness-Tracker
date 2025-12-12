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
using System.Net;
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

        // Firebase Web API Key used for Firestore REST calls (matches AuthService)
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
            // Create LocalDiaryEntry table for local diary storage
            await _database.CreateTableAsync<LocalDiaryEntry>();

            // Data Seeding: If the question bank is empty, we automatically fill it with default questions.
            if (await _database.Table<AssessmentQuestion>().CountAsync() == 0)
            {
                await SeedQuestionsAsync();
            }

            // Initialize Firestore SDK
            try
            {
                _firestoreDb = FirestoreDb.Create(ProjectId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Firestore Initialization Warning: {ex.Message}");
            }
        }

        // Diary core functions
        // 核心功能：添加日记 (离线优先 + REST API 上传)
        public async Task AddDiaryEntryAsync(LocalDiaryEntry localEntry)
        {
            await InitAsync();

            // 1. D步骤: 先保存到本地 SQLite (离线保护)
            // 无论有没有网，先存下来，保证数据不丢
            localEntry.IsSynced = false;
            await _database.InsertAsync(localEntry);

            // 2. 检查网络
            // 如果没网，或者没有 Token，就到此为止 (只存本地)
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
                // 3. 准备 REST API 请求
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Firestore REST API 创建文档的 URL
                string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries?key={WebApiKey}";

                // 4. A步骤: 构建数据包 (Payload)
                // Firestore REST API 要求特殊的 JSON 格式: { "fields": { "key": { "type": "value" } } }
                var firestorePayload = new
                {
                    fields = new
                    {
                        // 字符串类型用 stringValue
                        userId = new { stringValue = localEntry.UserId },
                        userEmail = new { stringValue = localEntry.UserEmail ?? "" }, // 防止 null
                        username = new { stringValue = localEntry.Username ?? "Anonymous" },
                        content = new { stringValue = localEntry.Content },

                        // 心情数据
                        moodName = new { stringValue = localEntry.MoodName },
                        moodEmoji = new { stringValue = localEntry.MoodEmoji },

                        // 注意：整数在 Firestore REST API 中必须转为字符串传给 integerValue
                        moodScore = new { integerValue = localEntry.MoodScore.ToString() },

                        // Social counters start at zero
                        likes = new { integerValue = "0" },
                        comments = new { integerValue = "0" },

                        // This will now send the actual cloud link (if the upload was successful)
                        imgUrl = new { stringValue = localEntry.ImgUrl ?? "" },

                        // 时间戳使用 timestampValue (ISO 8601 格式)
                        dateCreated = new { timestampValue = localEntry.DateCreated.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
                    }
                };

                // 序列化 JSON
                var jsonContent = JsonSerializer.Serialize(firestorePayload);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // 5. C步骤: 发送 POST 请求上传云端
                var response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    // 6. B步骤: 上传成功，更新本地状态

                    // 解析响应以获取 Firestore 生成的 ID
                    // 响应中包含 "name": "projects/.../documents/diary_entries/文档ID"
                    var responseBody = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(responseBody);
                    if (doc.RootElement.TryGetProperty("name", out var nameElement))
                    {
                        var path = nameElement.GetString();
                        // 截取最后一部分作为 ID
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

        public async Task<List<string>> GetAvailableTestTypesAsync()
        {
            await InitAsync();
            var records = await _database.Table<AssessmentQuestion>()
                                         .Select(q => q.TestType)
                                         .Distinct()
                                         .ToListAsync();
            return records;
        }

        public async Task<string> ChooseRandomTestTypeAsync()
        {
            var available = await GetAvailableTestTypesAsync();
            var pool = available?.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList() ?? new List<string>();

            if (pool.Count == 0)
            {
                return "PSS";
            }

            var random = new Random();
            var index = random.Next(pool.Count);
            return pool[index];
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

        // ✅ 核心新功能：从云端下载历史记录
        public async Task SyncAssessmentFromCloudAsync(string userId)
        {
            // 1. 检查网络和 Token
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return;
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2. 请求 Firestore 获取 assessments 集合中的所有文档
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

                // 3. 解析返回的文档列表
                if (doc.RootElement.TryGetProperty("documents", out JsonElement documents))
                {
                    await InitAsync(); // 确保本地数据库已就绪

                    foreach (var docElement in documents.EnumerateArray())
                    {
                        // 3.1 获取文档 ID (FirestoreId)
                        // path 格式: "projects/.../databases/(default)/documents/assessments/DOCUMENT_ID"
                        string path = docElement.GetProperty("name").GetString();
                        string firestoreId = path.Split('/').Last();

                        // 3.2 解析字段
                        var fields = docElement.GetProperty("fields");

                        // 辅助函数：安全读取 Firestore 字符串字段
                        string GetStr(string key) =>
                            fields.TryGetProperty(key, out var f) && f.TryGetProperty("stringValue", out var v) ? v.GetString() : "";

                        // 辅助函数：安全读取 Firestore 整数字段
                        int GetInt(string key) =>
                            fields.TryGetProperty(key, out var f) && f.TryGetProperty("integerValue", out var v) && int.TryParse(v.GetString(), out int i) ? i : 0;

                        // 检查这条记录是否属于当前用户
                        string recordUserId = GetStr("userId");
                        if (recordUserId != userId) continue; // 不是我的数据，跳过

                        // 3.3 检查本地是否已经存在 (防止重复添加)
                        var existing = await _database.Table<AssessmentResult>()
                                                      .Where(x => x.FirestoreId == firestoreId)
                                                      .FirstOrDefaultAsync();
                        if (existing != null) continue; // 本地已有，跳过

                        // 3.4 解析日期
                        DateTime dateTaken = DateTime.Now;
                        if (fields.TryGetProperty("dateTaken", out var dtField) && dtField.TryGetProperty("timestampValue", out var ts))
                        {
                            DateTime.TryParse(ts.GetString(), out dateTaken);
                        }

                        // 3.5 创建本地对象并保存
                        var newLocalResult = new AssessmentResult
                        {
                            FirestoreId = firestoreId,
                            UserId = recordUserId,
                            UserEmail = GetStr("userEmail"),
                            Username = GetStr("username"),
                            TestType = GetStr("testType"),
                            TotalScore = GetInt("totalScore"),
                            CalculatedResult = GetStr("calculatedResult"),
                            AnswersJson = GetStr("q_answer"), // 直接把 JSON 字符串存下来
                            DateTaken = dateTaken,
                            IsSynced = true // 既然是从云端下载的，肯定已同步
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
        public async Task<List<CloudDiaryEntry>> GetAllDiaryEntriesFromCloudAsync()
        {
            // Check the network
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return new List<CloudDiaryEntry>(); // No internet access, return empty list

            // Get Auth Token
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return new List<CloudDiaryEntry>();

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries?key={WebApiKey}";

            try
            {
                // Make the GET request
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var resultList = new List<CloudDiaryEntry>();

                    // Parse the JSON response
                    using (JsonDocument doc = JsonDocument.Parse(jsonString))
                    {
                        if (doc.RootElement.TryGetProperty("documents", out JsonElement documents))
                        {
                            foreach (var docElement in documents.EnumerateArray())
                            {
                                var fields = docElement.GetProperty("fields");

                                // Helper functions to extract values safely
                                string GetString(string key) =>
                                    fields.TryGetProperty(key, out var child) && child.TryGetProperty("stringValue", out var val) ? val.GetString() : "";

                                int GetInt(string key)
                                {
                                    if (fields.TryGetProperty(key, out var child) && child.TryGetProperty("integerValue", out var val))
                                    {
                                        return int.TryParse(val.GetString(), out var parsed) ? parsed : 0;
                                    }
                                    return 0;
                                }

                                string dateStr = GetString("dateCreated");
                                if (fields.TryGetProperty("dateCreated", out var dateField) && dateField.TryGetProperty("timestampValue", out var ts))
                                {
                                    dateStr = ts.GetString();
                                }

                                var firestoreId = docElement.TryGetProperty("name", out var nameProp)
                                    ? nameProp.GetString()?.Split('/').Last()
                                    : string.Empty;

                                resultList.Add(new CloudDiaryEntry
                                {
                                    Id = firestoreId,
                                    UserId = GetString("userId"),
                                    UserEmail = GetString("userEmail"),
                                    Username = GetString("username"),
                                    Content = GetString("content"),
                                    MoodEmoji = GetString("moodEmoji"),
                                    MoodName = GetString("moodName"),
                                    MoodScore = GetInt("moodScore"),
                                    ImgUrl = GetString("imgUrl"),
                                    Likes = GetInt("likes"),
                                    CommentsCount = GetInt("comments"),
                                    DateCreated = DateTime.TryParse(dateStr, out var dt) ? dt : DateTime.Now
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

            return new List<CloudDiaryEntry>();
        }

        public async Task<bool> UpdateDiaryEntryContentAsync(string firestoreId, string content)
        {
            if (string.IsNullOrEmpty(firestoreId)) return false;

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                fields = new
                {
                    content = new { stringValue = content }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var request = new HttpRequestMessage(new HttpMethod("PATCH"),
                $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries/{firestoreId}?key={WebApiKey}&updateMask.fieldPaths=content")
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteDiaryEntryAsync(string firestoreId)
        {
            if (string.IsNullOrEmpty(firestoreId)) return false;

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync(
                $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries/{firestoreId}?key={WebApiKey}");

            return response.IsSuccessStatusCode;
        }

        public async Task<PostComment> AddDiaryCommentAsync(string diaryId, PostComment comment)
        {
            if (string.IsNullOrEmpty(diaryId) || comment == null || string.IsNullOrWhiteSpace(comment.Text)) return null;

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return null;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                fields = new
                {
                    username = new { stringValue = comment.Username ?? "Anonymous" },
                    text = new { stringValue = comment.Text },
                    commentTime = new { timestampValue = comment.CommentTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ") }
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);

            var response = await client.PostAsync(
                $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries/{diaryId}/comments?key={WebApiKey}",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return null;

            return comment;
        }

        public async Task<List<PostComment>> GetDiaryCommentsAsync(string diaryId)
        {
            if (string.IsNullOrEmpty(diaryId)) return new List<PostComment>();

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return new List<PostComment>();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(
                $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries/{diaryId}/comments?key={WebApiKey}");

            if (!response.IsSuccessStatusCode) return new List<PostComment>();

            var jsonString = await response.Content.ReadAsStringAsync();
            var comments = new List<PostComment>();

            using var doc = JsonDocument.Parse(jsonString);
            if (doc.RootElement.TryGetProperty("documents", out var documents))
            {
                foreach (var docElement in documents.EnumerateArray())
                {
                    var fields = docElement.GetProperty("fields");

                    string GetString(string key) =>
                        fields.TryGetProperty(key, out var child) && child.TryGetProperty("stringValue", out var val) ? val.GetString() : string.Empty;

                    DateTime GetTime()
                    {
                        if (fields.TryGetProperty("commentTime", out var timeField) && timeField.TryGetProperty("timestampValue", out var ts))
                        {
                            if (DateTime.TryParse(ts.GetString(), out var parsed)) return parsed;
                        }
                        return DateTime.UtcNow;
                    }

                    comments.Add(new PostComment
                    {
                        Username = GetString("username"),
                        Text = GetString("text"),
                        CommentTime = GetTime()
                    });
                }
            }

            return comments.OrderByDescending(c => c.CommentTime).ToList();
        }

        public async Task<int?> UpdateDiaryLikesAsync(string diaryId, int newLikeCount)
        {
            if (string.IsNullOrEmpty(diaryId)) return null;

            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return null;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                fields = new
                {
                    likes = new { integerValue = newLikeCount.ToString() }
                }
            };

            var request = new HttpRequestMessage(new HttpMethod("PATCH"),
                $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/diary_entries/{diaryId}?key={WebApiKey}&updateMask.fieldPaths=likes")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            return newLikeCount;
        }

        public async Task<bool> SaveUserProfileAsync(UserProfile profile)
        {
            await InitAsync();
            try
            {
                profile.LastUpdated = DateTime.UtcNow;
                profile.IsSynced = false;

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

                // Try syncing to the backend
                _ = SyncUserProfileToCloudAsync(profile);
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
            var localProfile = await _database.Table<UserProfile>()
                                  .Where(p => p.UserId == userId)
                                  .FirstOrDefaultAsync();

            if (localProfile != null)
                return localProfile;

            var cloudProfile = await FetchUserProfileFromCloudAsync(userId);
            if (cloudProfile != null)
            {
                await _database.InsertAsync(cloudProfile);
                return cloudProfile;
            }

            return null;
        }

        private async Task SyncUserProfileToCloudAsync(UserProfile profile)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = new
                {
                    fields = new
                    {
                        userId = new { stringValue = profile.UserId },
                        username = new { stringValue = profile.Username ?? string.Empty },
                        bio = new { stringValue = profile.Bio ?? string.Empty },
                        profileImagePath = new { stringValue = profile.ProfileImagePath ?? string.Empty },
                        lastUpdated = new { timestampValue = profile.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ssZ") }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                string createUrl = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/user_profiles?documentId={profile.UserId}&key={WebApiKey}";
                var response = await client.PostAsync(createUrl, content);

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    string updateUrl = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/user_profiles/{profile.UserId}?key={WebApiKey}";
                    var patchRequest = new HttpRequestMessage(new HttpMethod("PATCH"), updateUrl)
                    {
                        Content = content
                    };
                    response = await client.SendAsync(patchRequest);
                }

                if (response.IsSuccessStatusCode)
                {
                    profile.IsSynced = true;
                    await _database.UpdateAsync(profile);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Profile sync failed: {error}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Profile Sync Exception: {ex.Message}");
            }
        }

        private async Task<UserProfile> FetchUserProfileFromCloudAsync(string userId)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return null;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string url = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/user_profiles/{userId}?key={WebApiKey}";
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;
                if (!root.TryGetProperty("fields", out var fields)) return null;

                string GetString(string key) =>
                    fields.TryGetProperty(key, out var f) && f.TryGetProperty("stringValue", out var v) ? v.GetString() : string.Empty;

                DateTime lastUpdated = DateTime.UtcNow;
                if (fields.TryGetProperty("lastUpdated", out var lastField) && lastField.TryGetProperty("timestampValue", out var ts))
                {
                    DateTime.TryParse(ts.GetString(), out lastUpdated);
                }

                return new UserProfile
                {
                    UserId = GetString("userId"),
                    Username = GetString("username"),
                    Bio = GetString("bio"),
                    ProfileImagePath = GetString("profileImagePath"),
                    IsSynced = true,
                    LastUpdated = lastUpdated
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Profile Download Exception: {ex.Message}");
                return null;
            }
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
