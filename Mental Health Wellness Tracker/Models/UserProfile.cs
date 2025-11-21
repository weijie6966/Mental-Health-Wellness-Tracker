using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class UserProfile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Bind to a specific UserID
        [Indexed] // Adding an index makes queries faster
        public string UserId { get; set; }

        public string Username { get; set; }
        public string Bio { get; set; }
        public string ProfileImagePath { get; set; }

        // Synchronization status
        public bool IsSynced { get; set; }

        // Last updated time (used to resolve multi-device conflicts, for future expansion).
        public DateTime LastUpdated { get; set; }
    }
}
