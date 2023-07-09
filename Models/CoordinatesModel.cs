using Google.Cloud.Firestore;

namespace Hikeyy.Models
{
    [FirestoreData]
    public class CoordinatesModel
    {
        [FirestoreProperty]
        public string? Latitude { get; set; }

        [FirestoreProperty]
        public string? Longitude { get; set; }

        [FirestoreProperty]
        public string? Name { get; set; }

        [FirestoreProperty]
        public int? Position { get; set; }
    }
}
