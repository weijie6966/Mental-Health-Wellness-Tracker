using System.Collections.Generic;
using System.Linq;

namespace Mental_Health_Wellness_Tracker.Models // Or .Services if you prefer
{
    // This static class holds the current, non-persisted state and history data.
    public static class AssessmentState
    {
        // 1. Recreates AssessmentResult.CurrentScore
        public static int CurrentScore { get; set; }

        // 2. Recreates AssessmentResult.QuestionScores (assuming it's the latest data)
        public static List<int> QuestionScores { get; set; } = new List<int>();

        // 3. Recreates AssessmentResult.History
        // This holds the list of persisted AssessmentResult objects.
        public static List<AssessmentResult> History { get; set; } = new List<AssessmentResult>();

        // 4. Recreates AssessmentResult.GetStatusMessage
        public static string GetStatusMessage(int score)
        {
            // NOTE: You may need to adjust the ranges based on your actual test
            if (score <= 15) return "Minimal Stress";
            if (score <= 30) return "Mild Stress";
            if (score <= 45) return "Moderate Stress";
            return "High Stress/Severe Distress";
        }
    }
}