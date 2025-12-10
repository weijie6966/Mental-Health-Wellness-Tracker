using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
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

        // Username and email of the user who took the assessment
        public string Username { get; set; }
        public string UserEmail { get; set; }

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

        [Ignore] // <-- Tells SQLite NOT to store this property
        public List<int> AnswerData
        {
            get
            {
                if (string.IsNullOrEmpty(AnswersJson))
                {
                    return new List<int>();
                }
                try
                {
                    // Deserialize the JSON string back into the usable list
                    return JsonSerializer.Deserialize<List<int>>(AnswersJson);
                }
                catch
                {
                    return new List<int>();
                }
            }
            set
            {
                // Serialize the list into a JSON string for storage
                AnswersJson = JsonSerializer.Serialize(value);
            }
        }
    }
}

