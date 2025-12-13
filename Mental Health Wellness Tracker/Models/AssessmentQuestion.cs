using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class AssessmentQuestion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // The type of assessment (currently "Rosenberg")
        public string TestType { get; set; }

        // The question text
        public string QuestionText { get; set; }

        // Whether to reverse the scoring (key logic!)
        public bool IsReversed { get; set; }

        // The maximum possible score for this question (Rosenberg uses 0-3)
        public int MaxScore { get; set; }

        // Order of the question in the assessment
        public int OrderIndex { get; set; }


        // --- NEW PROPERTY ADDED PER YOUR REQUEST ---
        // This holds the user's answer (MVVM state)
        // Note: This may conflict with your database structure if not handled by your repository.
        [Ignore] // Add [Ignore] if you don't want SQLite to try saving this property
        public int? SelectedScore { get; set; }
    }
}
