using System;
using System.Collections.Generic;

namespace Mental_Health_Wellness_Tracker.Models
{
    // --- GLOBAL STATE ---
    public static class AssessmentState
    {
        public static int CurrentScore { get; set; } = 0;

        // Helper for the current session
        public static List<int> QuestionScores { get; set; } = new List<int>();

        // The list of all past tests
        public static List<AssessmentHistoryItem> History { get; set; } = new List<AssessmentHistoryItem>();

        // Helper to determine status based on score
        public static string GetStatusMessage(int score)
        {
            if (score <= 15) return "Healthy";
            if (score <= 30) return "Mild Stress";
            if (score <= 45) return "Moderate";
            return "Needs Attention";
        }
    }
}