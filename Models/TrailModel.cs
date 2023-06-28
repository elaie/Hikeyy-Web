using Google.Cloud.Firestore;

namespace Hikeyy.Models
{
    [FirestoreData]
    public class TrailModel
    {
        [FirestoreProperty]
        public string? UID { get; set; }
        [FirestoreProperty]
        public List<string> BestMonths { get; set; }

        [FirestoreProperty]
        public int Budget { get; set; }

        [FirestoreProperty]
        public string Discription { get; set; }

        [FirestoreProperty]
        public int Duration { get; set; }

        [FirestoreProperty]
        public string Location { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public List<string>? PhotoURLs { get; set; }

        [FirestoreProperty]
        public string StartLatitude { get; set; }

        [FirestoreProperty]
        public string StartLongitude { get; set; }
        

    }

}
