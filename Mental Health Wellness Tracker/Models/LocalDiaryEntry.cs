using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class LocalDiaryEntry
    {
        // --- 1. Local Database Core ---
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } //

        // --- 2. Synchronize Control Fields ---
        // Mark this entry's ID in Firestore (to be filled in after upload)
        public string FirestoreId { get; set; }

        // Mark whether it has been synchronized (true = uploaded, false = to be uploaded)
        public bool IsSynced { get; set; }

        // --- 3. User Information ---
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Username { get; set; }

        // --- 4. Diary Contents ---
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }

        // --- 5. Mood and Media (Enhanced Functionality) ---
        public string MoodName { get; set; }
        public int MoodScore { get; set; }
        public string MoodEmoji { get; set; }
        public string ImgUrl { get; set; }
        // Local image path (stored in SQLite only, for offline uploading)
        public string LocalImagePath { get; set; }
    }
}
