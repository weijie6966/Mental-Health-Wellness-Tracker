using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class DiaryEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Associated users
        public string UserId { get; set; }

        // Username
        public string Username { get; set; }

        // Diary content
        public string Content { get; set; }

        // Mood (Save the emoji file as a filename, for example, "emoji_happy.png")
        public string MoodEmoji { get; set; }

        // Creation time
        public DateTime DateCreated { get; set; }

        // Has it been synced to the cloud
        public bool IsSynced { get; set; }
    }
}
