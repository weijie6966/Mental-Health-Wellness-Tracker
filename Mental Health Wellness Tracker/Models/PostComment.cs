using System;
// Add this if you want to include DateTime for comments
// using System.DateTime;

namespace Mental_Health_Wellness_Tracker.Models
{
    public class PostComment
    {
        public string Username { get; set; }
        public string Text { get; set; }
        // Recommended: Add Time for better context
        public DateTime CommentTime { get; set; }
    }
}