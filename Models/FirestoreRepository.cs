using FirebaseAdmin;
using Google.Cloud.Firestore;
using Google.Type;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hikeyy.Models
{
    public class FirestoreRepository<T>
    {
        private readonly FirestoreDb _db;
        private readonly string _collectionName;

        public FirestoreRepository(string projectId, string collectionName)
        {
            _db = FirestoreDb.Create(projectId);
            _collectionName = collectionName;
        }

        public async Task<string> CreateAsync(T entity)
        {
            var trailsCollection = _db.Collection(_collectionName);

            // Create a new document without specifying the document ID
            var newDocument = trailsCollection.Document();

            await newDocument.SetAsync(entity);

            //var docRef = await _db.Collection(_collectionName).AddAsync(entity);

            var generatedDocumentId = newDocument.Id;

           
            var updatedData = new Dictionary<string, object>
            {
                { "UID", generatedDocumentId }
               
            };
            await newDocument.UpdateAsync(updatedData);

            //System.Diagnostics.Debug.WriteLine("docREF"+docRef.Id);
            return newDocument.Id;
        }

        public async Task<T> GetAsync(string id)
        {
            var snapshot = await _db.Collection(_collectionName).Document(id).GetSnapshotAsync();
            if (snapshot.Exists)
                return snapshot.ConvertTo<T>();
            return default;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var query = _db.Collection(_collectionName);
            var snapshot = await query.GetSnapshotAsync();

            var entities = new List<T>();
            foreach (var document in snapshot.Documents)
            {
                System.Diagnostics.Debug.WriteLine(document.Id);
                entities.Add(document.ConvertTo<T>());
                System.Diagnostics.Debug.WriteLine(entities);

            }
            return entities;
        }

        public async Task UpdateAsync(string id, T entity)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            await docRef.SetAsync(entity, SetOptions.MergeAll);
        }

        public async Task DeleteAsync(string id)
        {
            var docRef = _db.Collection(_collectionName).Document(id);
            await docRef.DeleteAsync();
        }
    }
}
