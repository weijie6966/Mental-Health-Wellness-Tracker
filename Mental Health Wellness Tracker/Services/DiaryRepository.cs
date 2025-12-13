using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Google.Cloud.Firestore;
using Mental_Health_Wellness_Tracker.Models;
using SQLite;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Networking;

namespace Mental_Health_Wellness_Tracker.Services
{
    public class DiaryRepository
    {
        private FirestoreDb _firestoreDb;
        private SQLiteAsyncConnection _localDb;
        private const string ProjectId = "mental-health-wellness-tracker";

        public DiaryRepository()
        {
            // Initialize the local database (create tables)
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "MentalHealth.db3");
            _localDb = new SQLiteAsyncConnection(dbPath);
            _localDb.CreateTableAsync<LocalDiaryEntry>().Wait(); // Ensure table creation

            // Initialize Firestore
            try
            {
                _firestoreDb = FirestoreDb.Create(ProjectId);
            }
            catch
            {
                // Ignore initialization errors temporarily to prevent application crashes.
            }
        }

        // Add the parameter type to LocalDiaryEntry
        public async Task AddDiaryEntryAsync(LocalDiaryEntry localEntry)
        {
            // Save to local SQLite database
            localEntry.IsSynced = false; // Mark as not synced
            await _localDb.InsertAsync(localEntry);

            // Check network and Firestore connection
            if (Connectivity.NetworkAccess != NetworkAccess.Internet || _firestoreDb == null)
            {
                return; // The process ends when there's no internet connection. The data has already been saved locally.
            }

            try
            {
                // Model transformation (Local -> Cloud)
                var cloudEntry = new CloudDiaryEntry
                {
                    UserId = localEntry.UserId,
                    UserEmail = localEntry.UserEmail,
                    Username = localEntry.Username,
                    Content = localEntry.Content,
                    MoodName = localEntry.MoodName,
                    MoodScore = localEntry.MoodScore,
                    MoodEmoji = localEntry.MoodEmoji,
                    ImgUrl = localEntry.ImgUrl,
                    DateCreated = localEntry.DateCreated.ToUniversalTime()
                };

                // Upload to cloud Firestore
                CollectionReference collection = _firestoreDb.Collection("diary_entries");
                DocumentReference docRef = await collection.AddAsync(cloudEntry);

                // Update local status
                localEntry.FirestoreId = docRef.Id; // Store Firestore document ID
                localEntry.IsSynced = true;         // Mark as synced
                await _localDb.UpdateAsync(localEntry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sync failed: {ex.Message}");
            }
        }
    }
}
