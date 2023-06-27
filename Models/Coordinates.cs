using Google.Cloud.Firestore;

namespace Hikeyy.Models
{
    [FirestoreData]
    public class Coordinates
    {
        [FirestoreProperty]
        public string Long { get; set; }
    }
}
