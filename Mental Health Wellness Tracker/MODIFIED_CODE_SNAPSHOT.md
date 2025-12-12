## App.xaml.cs

```csharp
﻿using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Views;

namespace Mental_Health_Wellness_Tracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Start the app on the login page without relying on DI
            MainPage startPage = new MainPage();
            MainPage = new NavigationPage(startPage);
        }
    }
}
```

## MauiProgram.cs

```csharp
﻿using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<Mental_Health_Wellness_Tracker.App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("BrushFont.ttf", "BrushFont");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Comfortaa-Regular.ttf", "ComfortaaRegular");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    
}
```

## FILE_LINEUP.md

```csharp
# Project File Lineup

This repository layout excludes `Platforms`, `bin`, `obj`, `Resources`, and `Properties` as requested.

## Root
- App.xaml / App.xaml.cs
- FodyWeavers.xml / FodyWeavers.xsd
- MauiProgram.cs
- Mental Health Wellness Tracker.csproj
- Mental Health Wellness Tracker.csproj.user

## Models
- AssessmentQuestion.cs
- AssessmentResult.cs
- AssessmentState.cs
- CloudDiaryEntry.cs
- DiaryEntry.cs
- LocalDiaryEntry.cs
- Post.cs
- PostComment.cs
- UserProfile.cs

## Services
- AssessmentRepository.cs
- DiaryRepository.cs
- FirebaseStorageService.cs
- IAssessmentRepository.cs
- IAuthService.cs
- AuthService.cs

## ViewModels
- AnalyticViewModel.cs
- AssessmentDetailViewModel.cs
- AssessmentViewModel.cs
- CommunityViewModel.cs
- ContactUsViewModel.cs
- ForgotPasswordViewModel.cs
- MainViewModel.cs
- ProfileViewModel.cs
- RelayCommand.cs
- SignUpViewModel.cs
- ViewModelBase.cs
- WriteDiaryViewModel.cs

## Views
- AnalyticPage.xaml / AnalyticPage.xaml.cs
- AssessmentDetailPage.xaml / AssessmentDetailPage.xaml.cs
- AssessmentPage.xaml / AssessmentPage.xaml.cs
- CommunityPage.xaml / CommunityPage.xaml.cs
- ContactUsPage.xaml / ContactUsPage.xaml.cs
- ForgotPasswordPage.xaml / ForgotPasswordPage.xaml.cs
- MainPage.xaml / MainPage.xaml.cs
- ProfilePage.xaml / ProfilePage.xaml.cs
- ProfilePictureViewPage.xaml / ProfilePictureViewPage.xaml.cs
- SignUpPage.xaml / SignUpPage.xaml.cs
- SignUpSuccessPage.xaml / SignUpSuccessPage.xaml.cs
- WriteDiaryPage.xaml / WriteDiaryPage.xaml.cs

## ViewModel integration map
- **AnalyticViewModel.cs** → Services: `IAssessmentRepository` to load user assessment history from Firestore/SQLite; Models: `AssessmentResult` and answer data for bar-chart statistics and detail navigation.
- **AssessmentViewModel.cs** → Services: `IAssessmentRepository` (via `AssessmentRepository`) for downloading the question bank and persisting answers; Models: `AssessmentQuestion`, `AssessmentResult`; stores user context from secure storage before saving results with serialized answer data.
- **CommunityViewModel.cs** → Services: `IAssessmentRepository` for diary fetches plus cloud edit/delete/comment/like updates; Models: `CloudDiaryEntry` mapped into `Post`/`PostComment` view data.
- **MainViewModel.cs** → Services: `IAuthService` (via `AuthService`) for login/password validation.
- **ProfileViewModel.cs** → Services: `IAssessmentRepository` for profile load/save; Models: `UserProfile` with persisted avatar path and bio.
- **SignUpViewModel.cs** → Services: `IAuthService` (via `AuthService`) for registration and password validation.
- **WriteDiaryViewModel.cs** → Services: `IAssessmentRepository` for adding diary entries; Models: `LocalDiaryEntry` (mood, content, media paths) plus user metadata from secure storage.
- **ForgotPasswordViewModel.cs** → Services: `IAuthService` (via `AuthService`) to send Firebase reset emails and validate password strength before reset flow.

## Backend integration progress
- **Authentication flows**: login/sign-up/reset rely on `AuthService` for Firebase web API calls, secure storage of user tokens/IDs, and password validation; navigation stacks now instantiate view models directly without DI.
- **Assessments**: question downloads, answer submissions, and analytics all execute through `AssessmentRepository`, serializing answers with user context, syncing to Firestore with the shared API key, and exposing history for charts and detail screens.
- **Diary/community**: diary creation, edits, deletions, likes, and comments route through `AssessmentRepository` to the Firestore REST API; `CloudDiaryEntry`/`Post` models now include Firestore IDs and counters used by the community feed.
- **Profiles**: profile load/save first consult local storage, then Firestore, persisting avatars/bios and marking sync status when cloud updates succeed.

```

## Models/CloudDiaryEntry.cs

```csharp
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Mental_Health_Wellness_Tracker.Models
{
    // The [FirestoreData] tag tells Firestore that this is
    // a document object that can be uploaded
    [FirestoreData]
    public class CloudDiaryEntry
    {
        // Cloud primary key: Firestore automatically generated String ID
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string UserEmail { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public string Content { get; set; }

        [FirestoreProperty]
        public DateTime DateCreated { get; set; }

        [FirestoreProperty]
        public string MoodName { get; set; }

        [FirestoreProperty]
        public int MoodScore { get; set; }

        [FirestoreProperty]
        public string MoodEmoji { get; set; }

        [FirestoreProperty]
        public string ImgUrl { get; set; }

        [FirestoreProperty]
        public int Likes { get; set; }

        [FirestoreProperty]
        public int CommentsCount { get; set; }
    }
}

```

## Models/Post.cs

```csharp
﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models; // For PostComment reference

namespace Mental_Health_Wellness_Tracker.Models
{
    public class Post : INotifyPropertyChanged
    {
        // Data Fields
        public string FirestoreId { get; set; } // Cloud document id for edit/delete actions
        public string UserId { get; set; } // Added for deletion/editing checks
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MoodEmoji { get; set; } = "emoji_neutral.png";

        // Social Fields
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

        // INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

## Services/AssessmentRepository.cs

```csharp
﻿using System;
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

```

## Services/IAssessmentRepository.cs

```csharp
﻿using System;
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
        // User profile methods
        Task<bool> SaveUserProfileAsync(UserProfile profile);
        // Retrieve user profile by user ID
        Task<UserProfile> GetUserProfileAsync(string userId);
        // Sync assessments history from cloud to local database
        Task SyncAssessmentFromCloudAsync(string userId);
    }
}

```

## ViewModels/AnalyticViewModel.cs

```csharp
﻿using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Views;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class AnalyticViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository = new AssessmentRepository();
        private string _userId;

        // Properties bound to the View (Current Statistics)
        public string ScoreDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public string Recommendation { get; private set; }

        // Bar Chart Data (Heights and Counts)
        public double BarLowHeight { get; private set; }
        public double BarNormalHeight { get; private set; }
        public double BarHighHeight { get; private set; }
        public string CountLow { get; private set; }
        public string CountNormal { get; private set; }
        public string CountHigh { get; private set; }

        // History List
        // FIX 1: Change type to the unified AssessmentResult model
        public List<AssessmentResult> History { get; private set; }

        // Commands
        public ICommand ViewHistoryDetailCommand { get; }
        public ICommand NavigateCommand { get; }

        public AnalyticViewModel()
        {
            _ = InitializeAsync();

            // FIX 2: Change parameter type in RelayCommand to AssessmentResult
            ViewHistoryDetailCommand = new RelayCommand(async param =>
                await OnViewHistoryDetailClicked(param as AssessmentResult));

            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        private async Task InitializeAsync()
        {
            try
            {
                _userId = await SecureStorage.GetAsync("user_id");
                await LoadHistoryAsync();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to load analytics: {ex.Message}", "OK");
            }
        }

        // --- Core Logic ---

        public void LoadStatistics()
        {
            var latestResult = History?.FirstOrDefault();
            if (latestResult == null)
            {
                ScoreDisplay = "-";
                StatusDisplay = "No data yet";
                Recommendation = "Take your first assessment to see your stats.";
                ResetBarData();
                NotifyStatisticProperties();
                return;
            }

            int score = latestResult.TotalScore;
            ScoreDisplay = score.ToString();
            StatusDisplay = GetStatusMessage(score);

            if (score <= 15) Recommendation = "Great spot! Keep practicing self-care.";
            else if (score <= 30) Recommendation = "Mild stress. Get enough sleep.";
            else if (score <= 45) Recommendation = "Moderate stress. Use the Diary feature.";
            else Recommendation = "High distress. Please reach out to a professional.";

            var scores = latestResult.AnswerData;

            // Reset state if no scores are present
            if (scores == null || scores.Count == 0)
            {
                ResetBarData();
            }
            else
            {
                int low = scores.Count(s => s <= 1);
                int normal = scores.Count(s => s == 2);
                int high = scores.Count(s => s == 3);

                CountLow = low.ToString();
                CountNormal = normal.ToString();
                CountHigh = high.ToString();

                double m = 6.0;
                BarLowHeight = low * m;
                BarNormalHeight = normal * m * 150; // *150 added for visual scaling consistency
                BarHighHeight = high * m * 150;     // *150 added for visual scaling consistency
            }

            NotifyStatisticProperties();
        }

        private void ResetBarData()
        {
            BarLowHeight = 0;
            BarNormalHeight = 0;
            BarHighHeight = 0;
            CountLow = "0";
            CountNormal = "0";
            CountHigh = "0";
        }

        private void NotifyStatisticProperties()
        {
            OnPropertyChanged(nameof(ScoreDisplay));
            OnPropertyChanged(nameof(StatusDisplay));
            OnPropertyChanged(nameof(Recommendation));
            OnPropertyChanged(nameof(CountLow));
            OnPropertyChanged(nameof(CountNormal));
            OnPropertyChanged(nameof(CountHigh));
            OnPropertyChanged(nameof(BarLowHeight));
            OnPropertyChanged(nameof(BarNormalHeight));
            OnPropertyChanged(nameof(BarHighHeight));
        }

        public async Task LoadHistoryAsync()
        {
            if (string.IsNullOrEmpty(_userId))
            {
                History = new List<AssessmentResult>();
                OnPropertyChanged(nameof(History));
                return;
            }

            var results = await _repository.GetAssessmentHistoryAsync(_userId);
            History = results
                ?.OrderByDescending(x => x.DateTaken)
                .ToList()
                ?? new List<AssessmentResult>();

            OnPropertyChanged(nameof(History));
            LoadStatistics();
        }

        private async Task OnViewHistoryDetailClicked(AssessmentResult result)
        {
            if (result != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetailPage(result));
            }
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Stats") return;

            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "List" => new AssessmentPage(),
                "Diary" => new WriteDiaryPage(),
                "Profile" => new ProfilePage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }

        // Expose public method to call from page OnAppearing
        public async Task OnAppearingAsync()
        {
            await LoadHistoryAsync();
        }

        private string GetStatusMessage(int score)
        {
            if (score <= 15) return "Minimal Stress";
            if (score <= 30) return "Mild Stress";
            if (score <= 45) return "Moderate Stress";
            return "High Stress/Severe Distress";
        }
    }
}
```

## ViewModels/AssessmentViewModel.cs

```csharp
﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.Views;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Models;
using Microsoft.Maui.Storage;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // FIX: Class uses the base AssessmentQuestion model directly
    public class AssessmentViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _assessmentRepository = new AssessmentRepository();

        private const string DefaultTestType = "PSS";

        // FIX: ObservableCollection now holds the base AssessmentQuestion model
        public ObservableCollection<AssessmentQuestion> Questions { get; set; } = new ObservableCollection<AssessmentQuestion>();
        public AssessmentQuestion CurrentQuestion { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public string QuestionCounterDisplay => $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";
        public bool IsNotFirstQuestion => CurrentQuestionIndex > 0;
        public bool IsNotLastQuestion => CurrentQuestionIndex < Questions.Count - 1;
        public bool IsSubmitVisible => CurrentQuestionIndex == Questions.Count - 1;

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand SelectOptionCommand { get; }
        public ICommand SubmitCommand { get; }
        public ICommand NavigateCommand { get; }

        public AssessmentViewModel()
        {
            NextCommand = new RelayCommand(_ => MoveNext(), _ => CanMoveNext());
            PreviousCommand = new RelayCommand(_ => MovePrevious(), _ => CanMovePrevious());
            SelectOptionCommand = new RelayCommand<int>(score => SelectOption(score));
            SubmitCommand = new RelayCommand(async _ => await SubmitAssessment(), _ => CanSubmit());
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadQuestionsAsync();
            CurrentQuestion = Questions.FirstOrDefault();
            UpdateQuestionState();
        }

        private async Task LoadQuestionsAsync()
        {
            Questions.Clear();

            var fetched = await _assessmentRepository.GetQuestionsByTestTypeAsync(DefaultTestType);

            if (fetched != null && fetched.Count > 0)
            {
                foreach (var q in fetched)
                {
                    q.SelectedScore = null;
                    Questions.Add(q);
                }
            }
            else
            {
                // Fallback to a minimal built-in set if the repository is empty
                Questions.Add(new AssessmentQuestion { Id = 1, QuestionText = "I have been feeling down, depressed, or hopeless." });
                Questions.Add(new AssessmentQuestion { Id = 2, QuestionText = "I have had little interest or pleasure in doing things." });
                Questions.Add(new AssessmentQuestion { Id = 3, QuestionText = "I have had trouble falling or staying asleep, or sleeping too much." });
                Questions.Add(new AssessmentQuestion { Id = 4, QuestionText = "I have been feeling tired or having little energy." });
                Questions.Add(new AssessmentQuestion { Id = 5, QuestionText = "I have had poor appetite or overeating." });
            }

            CurrentQuestionIndex = 0;
        }

        private void SelectOption(int score)
        {
            if (CurrentQuestion != null)
            {
                // FIX: Uses the SelectedScore property directly from the model (now available)
                CurrentQuestion.SelectedScore = score;
                if (CanMoveNext())
                {
                    MoveNext();
                }
                else
                {
                    UpdateQuestionState();
                }
            }
        }

        private void MoveNext()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
            UpdateQuestionState();
        }

        private void UpdateQuestionState()
        {
            ((RelayCommand)NextCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PreviousCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SubmitCommand).RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(QuestionCounterDisplay));
            OnPropertyChanged(nameof(IsNotFirstQuestion));
            OnPropertyChanged(nameof(IsNotLastQuestion));
            OnPropertyChanged(nameof(IsSubmitVisible));
        }

        private bool CanMoveNext() => CurrentQuestionIndex < Questions.Count - 1 && CurrentQuestion.SelectedScore.HasValue;
        private void MovePrevious()
        {
            if (CurrentQuestionIndex > 0)
            {
                CurrentQuestionIndex--;
                CurrentQuestion = Questions[CurrentQuestionIndex];
            }
            UpdateQuestionState();
        }
        private bool CanMovePrevious() => CurrentQuestionIndex > 0;
        private bool CanSubmit() => CurrentQuestionIndex == Questions.Count - 1 && Questions.All(q => q.SelectedScore.HasValue);


        private async Task SubmitAssessment()
        {
            if (!CanSubmit()) return;

            int totalScore = Questions.Sum(q => q.SelectedScore.GetValueOrDefault());

            try
            {
                string userId = await SecureStorage.GetAsync("user_id");
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException("User not authenticated for assessment submission.");
                }

                var userEmail = await SecureStorage.GetAsync("user_email");
                var username = Preferences.Get("UsernameKey", "User");
                var answers = Questions.Select(q => q.SelectedScore ?? 0).ToList();

                // FIX: Uses properties from your AssessmentResult.cs (TotalScore, DateTaken, CalculatedResult)
                var result = new AssessmentResult
                {
                    UserId = userId,
                    UserEmail = userEmail,
                    Username = username,
                    TestType = DefaultTestType,
                    TotalScore = totalScore, // Correct property
                    DateTaken = DateTime.UtcNow, // Correct property
                    CalculatedResult = GetCalculatedResult(totalScore), // Correct property
                    AnswerData = answers
                };

                await _assessmentRepository.SaveAssessmentResultAsync(result);

                // Navigates to AssessmentDetailPage, passing the result object
                // Inside SubmitAssessment() method:
                await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new AssessmentDetailPage(result));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", $"Failed to submit assessment: {ex.Message}", "OK");
            }
        }

        // FIX: Method name changed to match the property name
        private string GetCalculatedResult(int score)
        {
            if (score <= 4) return "Minimal Depression";
            if (score <= 9) return "Mild Depression";
            if (score <= 14) return "Moderate Depression";
            if (score <= 19) return "Moderately Severe Depression";
            return "Severe Depression";
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;
            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
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
```

## ViewModels/CommunityViewModel.cs

```csharp
﻿using System;
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
```

## ViewModels/ForgotPasswordViewModel.cs

```csharp
﻿using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

        // Data Properties (Fody handles INPC)
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string VerificationCode { get; set; }

        // --- Visibility Toggles (Logic moved from code-behind) ---
        private bool _isNewPasswordVisible = false;
        public bool IsNewPasswordVisible
        {
            get => _isNewPasswordVisible;
            set
            {
                _isNewPasswordVisible = value;
                // Manually trigger updates for dependent properties
                OnPropertyChanged(nameof(ToggleNewPasswordImageSource));
                OnPropertyChanged(nameof(IsNewPasswordEntryHidden));
            }
        }

        private bool _isConfirmPasswordVisible = false;
        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set
            {
                _isConfirmPasswordVisible = value;
                // Manually trigger updates for dependent properties
                OnPropertyChanged(nameof(ToggleConfirmPasswordImageSource));
                OnPropertyChanged(nameof(IsConfirmPasswordEntryHidden));
            }
        }

        // Computed Properties
        public string ToggleNewPasswordImageSource => IsNewPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsNewPasswordEntryHidden => !IsNewPasswordVisible;

        public string ToggleConfirmPasswordImageSource => IsConfirmPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsConfirmPasswordEntryHidden => !IsConfirmPasswordVisible;

        // --- Commands ---
        public ICommand SendVerificationCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        public ICommand ToggleNewPasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand BackCommand { get; }

        public ForgotPasswordViewModel()
        {
            SendVerificationCommand = new RelayCommand(async _ => await OnSendVerificationClicked());
            ResetPasswordCommand = new RelayCommand(async _ => await OnResetPasswordClicked());
            ToggleNewPasswordCommand = new RelayCommand(OnToggleNewPasswordClicked);
            ToggleConfirmPasswordCommand = new RelayCommand(OnToggleConfirmPasswordClicked);
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());
        }

        // --- Core Logic (Moved from ForgotPasswordPage.xaml.cs) ---

        private async Task OnSendVerificationClicked()
        {
            string inputEmail = Email?.Trim();

            if (string.IsNullOrWhiteSpace(inputEmail))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            try
            {
                await _authService.SendPasswordResetEmailAsync(inputEmail);
                await Application.Current.MainPage.DisplayAlert("Sent", $"Password reset link sent to {inputEmail}. Please check your inbox.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task OnResetPasswordClicked()
        {
            string email = Email?.Trim();
            string newPass = NewPassword;
            string confirmPass = ConfirmPassword;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (newPass != confirmPass)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New passwords do not match.", "OK");
                return;
            }

            if (!_authService.IsPasswordValid(newPass))
            {
                await Application.Current.MainPage.DisplayAlert("Weak Password",
                    "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number",
                    "OK");
                return;
            }

            try
            {
                await _authService.SendPasswordResetEmailAsync(email);
                await Application.Current.MainPage.DisplayAlert("Check Your Email", "We sent you a reset link. Follow it to set your new password.", "OK");
                await Application.Current.MainPage.Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void OnToggleNewPasswordClicked(object parameter)
        {
            IsNewPasswordVisible = !IsNewPasswordVisible;
        }

        private void OnToggleConfirmPasswordClicked(object parameter)
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }
    }
}
```

## ViewModels/MainViewModel.cs

```csharp
﻿using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Services;
using Mental_Health_Wellness_Tracker.Views;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

        // Data Properties bound to the View
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsPasswordVisible { get; set; } = false;

        // Computed Properties (Handle visual changes)
        public string TogglePasswordImageSource => IsPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsPasswordEntryHidden => !IsPasswordVisible;

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand CreateAccountCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        // REMOVED: SocialLoginCommand is deleted

        // NEW: Command to handle taps on unimplemented features (social buttons)
        public ICommand UnimplementedCommand { get; }

        public MainViewModel()
        {
            LoginCommand = new RelayCommand(async _ => await OnLoginClicked());
            TogglePasswordCommand = new RelayCommand(OnTogglePasswordClicked);

            CreateAccountCommand = new RelayCommand(async _ => await OnNavTapped(nameof(SignUpPage)));
            ForgotPasswordCommand = new RelayCommand(async _ => await OnNavTapped(nameof(ForgotPasswordPage)));

            // NEW: Initialize the command for unimplemented features
            UnimplementedCommand = new RelayCommand(async param => await OnUnimplementedClicked(param?.ToString()));
        }

        // --- Core Authentication Logic ---

        private async Task OnLoginClicked()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            // Validation uses the injected service
            if (!_authService.IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Password must contain Upper, Lower, and Number.", "OK");
                return;
            }

            try
            {
                // Login uses the injected service
                string userId = await _authService.LoginAsync(Email, Password);

                // Navigate to the next page (ProfilePage) using DI
                await OnNavTapped(nameof(ProfilePage));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Login Failed", $"Error: {ex.Message}", "Try Again");
            }
        }

        // NEW: Logic for handling taps on unimplemented social buttons (Google, Apple, etc.)
        private async Task OnUnimplementedClicked(string featureName)
        {
            if (string.IsNullOrEmpty(featureName)) return;

            await Application.Current.MainPage.DisplayAlert(
                "Future Update",
                $"The {featureName} feature is currently in development and will be available in a future update.",
                "OK");
        }

        // --- UI Interaction Logic ---

        private void OnTogglePasswordClicked(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
            OnPropertyChanged(nameof(TogglePasswordImageSource));
            OnPropertyChanged(nameof(IsPasswordEntryHidden));
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;

            Page nextPage = destination switch
            {
                nameof(SignUpPage) => new SignUpPage(),
                nameof(ForgotPasswordPage) => new ForgotPasswordPage(),
                nameof(ProfilePage) => new ProfilePage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}
```

## ViewModels/ProfileViewModel.cs

```csharp
﻿using System;
using System.IO;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Views;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // Assuming Fody.PropertyChanged or manual INPC for UI updates
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository = new AssessmentRepository();
        private UserProfile _userProfile; // Model to hold profile data

        // FIX: Properties now rely on the _userProfile model
        public string Username
        {
            get => _userProfile.Username;
            set { _userProfile.Username = value; OnPropertyChanged(); }
        }
        public string Bio
        {
            get => _userProfile.Bio;
            set { _userProfile.Bio = value; OnPropertyChanged(); }
        }

        public ImageSource ProfileAvatarSource { get; set; } = "nav_profile.png"; // Default image
        public bool IsMenuVisible { get; set; } = false;

        // Commands (initialization remains the same)
        public ICommand SaveProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ChangeProfilePictureCommand { get; }
        public ICommand ViewProfilePictureCommand { get; }
        public ICommand ToggleMenuCommand { get; }
        public ICommand NavigateCommand { get; }
        public ICommand ContactUsCommand { get; }
        public ICommand AppearingCommand { get; } // For loading data OnAppearing

        public ProfileViewModel()
        {
            _userProfile = new UserProfile();

            // Initialize Commands
            SaveProfileCommand = new RelayCommand(async _ => await OnSaveProfileClicked());
            LogoutCommand = new RelayCommand(async _ => await OnLogoutClicked());
            ChangeProfilePictureCommand = new RelayCommand(async _ => await OnChangeProfileClicked());
            ViewProfilePictureCommand = new RelayCommand(async _ => await OnViewProfileClicked());
            ToggleMenuCommand = new RelayCommand(OnToggleMenuClicked);
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
            ContactUsCommand = new RelayCommand(async _ => await OnContactUsClicked());
            AppearingCommand = new RelayCommand(async _ => await LoadProfileData());

            // Run initial load (OnAppearing will trigger the full reload later)
            Task.Run(LoadProfileData);
        }

        // --- Data Loading & Saving (Business Logic) ---

        public async Task LoadProfileData() // FIX 2: Load from repository
        {
            string userId = await SecureStorage.GetAsync("user_id");
            if (string.IsNullOrEmpty(userId)) return;

            // Fetch data from repository
            var profile = await _repository.GetUserProfileAsync(userId);

            if (profile != null)
            {
                _userProfile = profile;

                // Update local preference cache (for compatibility with WriteDiaryViewModel)
                Preferences.Set("UsernameKey", profile.Username);

                // Trigger UI update for bound properties
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(Bio));

                // Load and set avatar source
                if (!string.IsNullOrEmpty(_userProfile.ProfileImagePath) && File.Exists(_userProfile.ProfileImagePath))
                {
                    ProfileAvatarSource = ImageSource.FromFile(_userProfile.ProfileImagePath);
                }
                else
                {
                    ProfileAvatarSource = "nav_profile.png";
                }
                OnPropertyChanged(nameof(ProfileAvatarSource));
            }
        }

        private async Task OnSaveProfileClicked() // FIX 3: Save to repository
        {
            // The properties (Username, Bio) are already updated via the setters,
            // so we just need to update the remaining model fields and persist.
            _userProfile.LastUpdated = DateTime.Now;

            bool success = await _repository.SaveUserProfileAsync(_userProfile);
            if (success)
            {
                // Update local preference cache (for compatibility with WriteDiaryViewModel)
                Preferences.Set("UsernameKey", _userProfile.Username);
                await Application.Current.MainPage.DisplayAlert("Success", "Profile updated successfully!", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update profile.", "OK");
            }
        }

        // --- Image/File Logic ---

        private async Task OnChangeProfileClicked()
        {
            IsMenuVisible = false;

            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Select a profile picture", FileTypes = FilePickerFileType.Images });

                if (result != null)
                {
                    string permanentPath = Path.Combine(FileSystem.AppDataDirectory, "user_profile_pic.png");

                    // Copy file to permanent storage location
                    using (var sourceStream = await result.OpenReadAsync())
                    using (var localFileStream = File.Create(permanentPath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    }

                    // Update model and UI
                    _userProfile.ProfileImagePath = permanentPath;
                    ProfileAvatarSource = ImageSource.FromFile(permanentPath);
                    OnPropertyChanged(nameof(ProfileAvatarSource));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not pick image: {ex.Message}", "OK");
            }
        }

        private async Task OnViewProfileClicked()
        {
            IsMenuVisible = false;

            await Application.Current.MainPage.Navigation.PushAsync(new ProfilePictureViewPage(_userProfile.ProfileImagePath));
        }

        // --- Command Logic ---

        private void OnToggleMenuClicked(object parameter)
        {
            IsMenuVisible = !IsMenuVisible;
        }

        private async Task OnLogoutClicked()
        {
            // Clear session data
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");
            Preferences.Clear();

            await Application.Current.MainPage.DisplayAlert("Logout", "You have been logged out.", "OK");

            // Navigate to the root (Login) screen
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        // FIX 5: Navigation method for Contact Us page (uses DI)
        private async Task OnContactUsClicked()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ContactUsPage());
        }

        // FIX 6: All bottom navigation uses DI
        private async Task OnNavTapped(string destination)
        {
            if (destination == null || destination == "Profile") return;

            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "List" => new AssessmentPage(),
                "Diary" => new WriteDiaryPage(),
                "Stats" => new AnalyticPage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}
```

## ViewModels/SignUpViewModel.cs

```csharp
﻿using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Services; // FIX 1: Add Services for IAuthService
using Mental_Health_Wellness_Tracker.Views;       // FIX 2: Add Views for DI Navigation

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    public class SignUpViewModel : ViewModelBase
    {
        private readonly IAuthService _authService = new AuthService();

        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        // --- Password Visibility Properties (Logic remains in ViewModel for UI state) ---
        private bool _isPasswordVisible = false;
        public bool IsPasswordVisible { get => _isPasswordVisible; set { _isPasswordVisible = value; OnPropertyChanged(nameof(TogglePasswordImageSource)); OnPropertyChanged(nameof(IsPasswordEntryHidden)); } }
        private bool _isConfirmPasswordVisible = false;
        public bool IsConfirmPasswordVisible { get => _isConfirmPasswordVisible; set { _isConfirmPasswordVisible = value; OnPropertyChanged(nameof(ToggleConfirmPasswordImageSource)); OnPropertyChanged(nameof(IsConfirmPasswordEntryHidden)); } }

        public string TogglePasswordImageSource => IsPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsPasswordEntryHidden => !IsPasswordVisible;
        public string ToggleConfirmPasswordImageSource => IsConfirmPasswordVisible ? "eye_closed.png" : "eye_open.png";
        public bool IsConfirmPasswordEntryHidden => !IsConfirmPasswordVisible;

        // Commands
        public ICommand SignUpCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand SocialLoginCommand { get; }


        public SignUpViewModel()
        {
            SignUpCommand = new RelayCommand(async _ => await OnSignUpClicked());
            TogglePasswordCommand = new RelayCommand(OnTogglePasswordClicked);
            ToggleConfirmPasswordCommand = new RelayCommand(OnToggleConfirmPasswordClicked);

            // FIX 6: Back command uses DI navigation helper
            BackCommand = new RelayCommand(async _ => await Application.Current.MainPage.Navigation.PopAsync());

            SocialLoginCommand = new RelayCommand(async parameter => await OnSocialLoginClicked(parameter?.ToString()));
        }

        // --- Core Logic (Refactored to use IAuthService) ---

        private async Task OnSignUpClicked()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // Validation (Some checks remain here, others move to service)
            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            // FIX 7: Password validation uses the injected service
            if (!_authService.IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Weak Password", "Password must contain at least:\n- One Uppercase letter\n- One Lowercase letter\n- One Number", "OK");
                return;
            }

            // FIX 8: Registration uses the injected service
            try
            {
                // Note: The email validation (ending in @gmail.com) is now assumed to be handled either
                // by Firebase rules (if using Firebase) or inside the IAuthService implementation.
                string userId = await _authService.SignUpAsync(Email, Password);

                await Application.Current.MainPage.DisplayAlert("Success", "Account created successfully!", "OK");

                // FIX 9: Navigate to success page using DI
                await OnNavTapped(nameof(SignUpSuccessPage));
            }
            catch (Exception ex)
            {
                // Display error message provided by the AuthService (e.g., Email already in use)
                await Application.Current.MainPage.DisplayAlert("Registration Failed", ex.Message, "OK");
            }
        }

        // --- Toggle Logic (Remains in ViewModel for UI state) ---

        private void OnTogglePasswordClicked(object parameter)
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void OnToggleConfirmPasswordClicked(object parameter)
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }

        // --- Social Login Logic ---

        private async Task OnSocialLoginClicked(string provider)
        {
            if (string.IsNullOrEmpty(provider)) return;

            await Application.Current.MainPage.DisplayAlert(
                "Future Update",
                $"The {provider} login feature is currently in development and will be available in a future update.",
                "OK");
        }

        private async Task OnNavTapped(string destination)
        {
            Page nextPage = destination switch
            {
                nameof(SignUpSuccessPage) => new SignUpSuccessPage(),
                nameof(SignUpPage) => new SignUpPage(),
                nameof(ForgotPasswordPage) => new ForgotPasswordPage(),
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}
```

## ViewModels/WriteDiaryViewModel.cs

```csharp
﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic; // Required for List<byte[]>
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.Views;
using Mental_Health_Wellness_Tracker.Services;

namespace Mental_Health_Wellness_Tracker.ViewModels
{
    // Assuming Fody.PropertyChanged is used or you implement INPC manually
    public class WriteDiaryViewModel : ViewModelBase
    {
        private readonly IAssessmentRepository _repository = new AssessmentRepository();

        // Data fields for the UI (using the MVVM pattern)
        public string DiaryEntryText { get; set; } // Binds to Text in XAML
        public ObservableCollection<ImageSource> SelectedImages { get; set; } = new ObservableCollection<ImageSource>();
        public string SelectedMoodEmoji { get; set; } = "emoji_neutral.png";

        // FIX 2: List to hold local paths, as per original logic (only one image in original logic)
        private string _selectedImagePath = string.Empty;
        public string SelectedImagePathDisplay { get; private set; } = "No file chosen";

        // --- Commands ---
        public ICommand MoodEmojiClickedCommand { get; }
        public ICommand UploadImagesCommand { get; }
        public ICommand PostCommand { get; }
        public ICommand NavigateCommand { get; }

        public WriteDiaryViewModel()
        {
            MoodEmojiClickedCommand = new RelayCommand(OnMoodEmojiClicked);
            UploadImagesCommand = new RelayCommand(async _ => await OnUploadClicked()); // Renamed
            PostCommand = new RelayCommand(async _ => await OnPostClicked());
            NavigateCommand = new RelayCommand(async param => await OnNavTapped(param?.ToString()));
        }

        // --- Logic (Transferred from Code-Behind) ---

        private void OnMoodEmojiClicked(object parameter)
        {
            // The XAML needs to be bound to this method
            if (parameter is string emojiFileName)
            {
                SelectedMoodEmoji = emojiFileName;
                // NOTE: Border highlighting logic should be moved to XAML using DataTriggers/Converters
            }
        }

        private async Task OnUploadClicked() // Adjusted to match original single-file upload logic
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a photo",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // 1. Store path for persistence model
                    _selectedImagePath = result.FullPath;

                    // 2. Update display property
                    SelectedImagePathDisplay = result.FileName;
                    OnPropertyChanged(nameof(SelectedImagePathDisplay));

                    // 3. Update preview (Optional: If you want to show the image preview in the UI)
                    // We clear and add the new image source
                    SelectedImages.Clear();
                    SelectedImages.Add(ImageSource.FromFile(result.FullPath));
                }
            }
            catch (Exception)
            {
                // Ignore cancellation or error
            }
        }

        private async Task OnPostClicked()
        {
            if (string.IsNullOrWhiteSpace(DiaryEntryText) && string.IsNullOrEmpty(_selectedImagePath))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Hold On",
                    "Please write your diary entry before posting.",
                    "OK"
                );
                return;
            }

            var userId = await SecureStorage.GetAsync("user_id");
            var userEmail = await SecureStorage.GetAsync("user_email");
            var username = Preferences.Get("UsernameKey", "User");

            if (string.IsNullOrEmpty(userId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in!", "OK");
                return;
            }

            // Get mood details using the helper method
            (string moodName, int moodScore) = GetMoodDetails(SelectedMoodEmoji);

            // ✅ Map data to LocalDiaryEntry (persistence model)
            var newEntry = new LocalDiaryEntry
            {
                UserId = userId,
                UserEmail = userEmail,
                Username = username,
                Content = DiaryEntryText,

                MoodEmoji = SelectedMoodEmoji,
                MoodName = moodName,
                MoodScore = moodScore,

                ImgUrl = "", // Cloud URL is empty until background sync runs
                LocalImagePath = _selectedImagePath, // Store the local path
                DateCreated = DateTime.Now,
                IsSynced = false
            };

            // ✅ Call AddDiaryEntryAsync on the injected repository
            await _repository.AddDiaryEntryAsync(newEntry);

            await Application.Current.MainPage.DisplayAlert("Success", "Diary saved locally! Syncing in background...", "OK");

            // Clean up the UI properties
            DiaryEntryText = string.Empty;
            SelectedImagePathDisplay = "No file chosen";
            _selectedImagePath = string.Empty;
            SelectedImages.Clear();

            await Application.Current.MainPage.Navigation.PushAsync(new CommunityPage());
        }

        private (string name, int score) GetMoodDetails(string emojiFile)
        {
            switch (emojiFile)
            {
                case "emoji_dead.png": return ("Super unhappy", 1);
                case "emoji_sad.png": return ("unhappy", 2);
                case "emoji_neutral.png": return ("normal", 3);
                case "emoji_smile.png": return ("happy", 4);
                case "emoji_love.png": return ("super happy", 5);
                default: return ("normal", 3);
            }
        }

        private async Task OnNavTapped(string destination)
        {
            if (destination == null) return;

            Page nextPage = destination switch
            {
                "Community" => new CommunityPage(),
                "List" => new AssessmentPage(),
                "Stats" => new AnalyticPage(),
                "Profile" => new ProfilePage(),
                "Diary" => null, // Current page
                _ => null
            };

            if (nextPage != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(nextPage);
            }
        }
    }
}
```

## Views/AnalyticPage.xaml.cs

```csharp
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AnalyticPage : ContentPage
    {
        private readonly AnalyticViewModel _viewModel;

        public AnalyticPage()
        {
            InitializeComponent();
            _viewModel = new AnalyticViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.OnAppearingAsync();
        }
    }
}

```

## Views/AssessmentPage.xaml.cs

```csharp
using Microsoft.Maui.Controls;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class AssessmentPage : ContentPage
    {
        public AssessmentPage()
        {
            InitializeComponent();
            BindingContext = new AssessmentViewModel();
        }
    }
}

```

## Views/CommunityPage.xaml.cs

```csharp
using Microsoft.Maui.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Mental_Health_Wellness_Tracker.Models;
using Mental_Health_Wellness_Tracker.ViewModels;

namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class CommunityPage : ContentPage, INotifyPropertyChanged
    {
        private readonly CommunityViewModel _viewModel;

        public CommunityPage()
        {
            InitializeComponent();
            _viewModel = new CommunityViewModel();
            BindingContext = _viewModel;
        }

        public CommunityPage(Post newEntry) : this()
        {
            if (newEntry != null)
            {
                _viewModel.AddNewPost(newEntry);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

```

## Views/MainPage.xaml.cs

```csharp
using Mental_Health_Wellness_Tracker.ViewModels;
using Microsoft.Maui.Controls;

// Assuming your pages are in the Views namespace
namespace Mental_Health_Wellness_Tracker.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
}

```

## Views/ProfilePage.xaml.cs

```csharp
using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    // The code-behind for ProfilePage is now clean.
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.ProfileViewModel();
        }

        // All fields, data loading, saving, and navigation logic are moved to the ViewModel.
    }
}
```

## Views/SignUpPage.xaml.cs

```csharp
using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    // The code-behind for SignUpPage is now clean.
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.SignUpViewModel();
        }

        // All logic and event handlers have been moved to SignUpViewModel.cs.
        // The binding context is now set in SignUpPage.xaml.
    }
}
```

## Views/WriteDiaryPage.xaml.cs

```csharp
using Microsoft.Maui.Controls;

namespace Mental_Health_Wellness_Tracker
{
    // The classes Post and PostComment are moved to Models/PostModels.cs

    // All logic is moved to WriteDiaryViewModel.
    public partial class WriteDiaryPage : ContentPage
    {
        public WriteDiaryPage()
        {
            InitializeComponent();
            BindingContext = new ViewModels.WriteDiaryViewModel();
        }
    }
}
```

