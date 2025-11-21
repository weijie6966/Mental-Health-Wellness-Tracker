using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class AssessmentResult
    {
        // SQLite local primary key, auto-incrementing
        [PrimaryKey, AutoIncrement]
        public int LocalId { get; set; }

        // Document IDs on Firebase (used for synchronization identification)
        public string FirestoreId { get; set; }

        // User ID to link the result to a specific user
        public string UserId { get; set; }

        // Test type (e.g., "PSS" or "Rosenberg")
        public string TestType { get; set; }

        // Date when the assessment was taken
        public DateTime DateTaken { get; set; }

        // Total score achieved in the assessment
        public int TotalScore { get; set; }

        // Calculated result based on the score (e.g., "Moderate Stress")
        public string CalculatedResult { get; set; }

        // Synchronization status with remote database
        // True if synced, false otherwise
        public bool IsSynced { get; set; }

        // Stores a JSON string containing the detailed answer (to simplify SQLite storage).
        public string AnswersJson { get; set; }
    }
}
