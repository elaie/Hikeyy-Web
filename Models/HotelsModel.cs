using Google.Cloud.Firestore;

namespace Hikeyy.Models
{
    [FirestoreData]
    public class HotelsModel
    {
        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public string? Phone { get; set; }

        [FirestoreProperty]
        public string? Location { get; set; }
    }
}
