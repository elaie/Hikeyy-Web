using Google.Cloud.Firestore;
using Hikeyy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Hikeyy.Controllers
{
    public class CoordinatesController : Controller
    {
        private readonly FirestoreRepository<TrailModel> _trailRepository;
        FirestoreDb _db;

        // GET: CoordinatesController
        public CoordinatesController()
        {
            System.Diagnostics.Debug.WriteLine("COORDINATES KO CONSTRUCTOR CALLED HAI");
            var credentialsPath = "hikeyyy-firebase-adminsdk-oj70g-ca14e13827.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            // Initialize Firestore client
            _db = FirestoreDb.Create("hikeyyy");
        }
        public async Task<IActionResult> Index(string id)
        {
            string parentCollection = "Trails";
            string parentDocumentId = id;
            // Specify the new collection name
            string newCollectionName = "Cordinates";

            DocumentReference parentDocumentRef = _db.Collection(parentCollection).Document(parentDocumentId);

            CollectionReference newCollectionRef = parentDocumentRef.Collection(newCollectionName);

            QuerySnapshot snapshot = await newCollectionRef.GetSnapshotAsync();

            List<CoordinatesModel> products = new List<CoordinatesModel>();

            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                // Map Firestore document data to a Product model object

                CoordinatesModel Coordinates = new CoordinatesModel
                {
                    Longitude = documentSnapshot.GetValue<string>("Longitude"),
                    Latitude = documentSnapshot.GetValue<string>("Latitude"),
                    Name = documentSnapshot.GetValue<string>("Name"),
                    Position = documentSnapshot.GetValue<int>("pos"),
                    
                    // Map more properties as needed
                };

                products.Add(Coordinates);
            }

            return View(products);
        }
       
        // GET: CoordinatesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CoordinatesController/Create
        public ActionResult Create()
        {
            string myString = (string)TempData["TrailUID"];
            System.Diagnostics.Debug.WriteLine($"Created {myString}");
            ViewBag.MyString = myString;
            //System.Diagnostics.Debug.WriteLine("INSIDE CREATE" + id);
            return View();
        }

        // POST: CoordinatesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoordinatesModel Coordinate, string myString)
        {
            
            try
            {
                System.Diagnostics.Debug.WriteLine("INSIDE CREATE POST REQUEST");
                // Specify the parent collection and document ID
                string parentCollection = "Trails";
                string parentDocumentId = myString;
                // Specify the new collection name
                string newCollectionName = "Cordinates";

                DocumentReference parentDocumentRef = _db.Collection(parentCollection).Document(parentDocumentId);

                // Create a new document data with an empty field to trigger the creation of a sub-collection
                var data = new Dictionary<string, object>
                {
                    { "Latitude", Coordinate.Latitude },
                    { "Longitude", Coordinate.Longitude },
                    { "Name", Coordinate.Name },
                    { "pos", Coordinate.Position },

                };
                // Access the new collection reference within the parent document
                CollectionReference newCollectionRef = parentDocumentRef.Collection(newCollectionName);

                await newCollectionRef.AddAsync(data);

                return RedirectToAction(nameof(Index), new { id = myString });
            }
            catch
            {
                return View();
            }
        }

        // GET: CoordinatesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CoordinatesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CoordinatesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CoordinatesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
