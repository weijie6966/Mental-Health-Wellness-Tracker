using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Mental_Health_Wellness_Tracker.Models
{
    // The [FirestoreData] tag tells Firestore that this is
    // a document object that can be uploaded
    [FirestoreData]
    public class CloudDiaryEntry
    {
        // Cloud primary key: Firestore automatically generated String ID
        [FirestoreDocumentId]
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string UserEmail { get; set; }

        [FirestoreProperty]
        public string Username { get; set; }

        [FirestoreProperty]
        public string Content { get; set; }

        [FirestoreProperty]
        public DateTime DateCreated { get; set; }

        [FirestoreProperty]
        public string MoodName { get; set; }

        [FirestoreProperty]
        public int MoodScore { get; set; }

        [FirestoreProperty]
        public string MoodEmoji { get; set; }

        [FirestoreProperty]
        public string ImgUrl { get; set; }

        [FirestoreProperty]
        public int CommentsCount { get; set; }
    }
}
