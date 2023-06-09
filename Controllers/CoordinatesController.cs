﻿using Google.Cloud.Firestore;
using Google.Type;
using Hikeyy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Xml.Linq;

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
            ViewData["UID"] = id;
            System.Diagnostics.Debug.WriteLine(ViewData["UID"]);
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
        public async Task<IActionResult> Details(string id, string name)
        {
            string parentCollection = "Trails";
            string parentDocumentId = id;
            // Specify the new collection name
            string newCollectionName = "Cordinates";

            DocumentReference parentDocumentRef = _db.Collection(parentCollection).Document(parentDocumentId);

            CollectionReference newCollectionRef = parentDocumentRef.Collection(newCollectionName);

            QuerySnapshot snapshot = await newCollectionRef.GetSnapshotAsync();


            List<CoordinatesModel> products = new List<CoordinatesModel>();
            ViewData["UID"] = id;
            System.Diagnostics.Debug.WriteLine(ViewData["UID"] + " " + name);
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                // Map Firestore document data to a Product model object
                if (name == documentSnapshot.GetValue<string>("Name"))
                {
                    CoordinatesModel Coordinates = new CoordinatesModel
                    {
                        Longitude = documentSnapshot.GetValue<string>("Longitude"),
                        Latitude = documentSnapshot.GetValue<string>("Latitude"),
                        Name = documentSnapshot.GetValue<string>("Name"),
                        Position = documentSnapshot.GetValue<int>("pos"),

                        // Map more properties as needed
                    };
                    return View(Coordinates);
                }


            }
            return View();
        }

        // GET: CoordinatesController/Create
        public ActionResult Create(string id)
        {
            
            System.Diagnostics.Debug.WriteLine($"Created {id}");
            ViewData["UID"] = id;
            //ViewBag.MyString = myString;
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
        public async Task<IActionResult> Edit(string id,string name)
        {
            string parentCollection = "Trails";
            string parentDocumentId = id;
            // Specify the new collection name
            string newCollectionName = "Cordinates";
            TempData["Name"]  = Request.Query["name"];
            TempData["id"] = Request.Query["id"];
            DocumentReference parentDocumentRef = _db.Collection(parentCollection).Document(parentDocumentId);

            CollectionReference newCollectionRef = parentDocumentRef.Collection(newCollectionName);

            QuerySnapshot snapshot = await newCollectionRef.GetSnapshotAsync();


            List<CoordinatesModel> products = new List<CoordinatesModel>();
            ViewData["UID"] = id;
            System.Diagnostics.Debug.WriteLine(ViewData["UID"]+" "+ TempData["Name"]);
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                // Map Firestore document data to a Product model object
                if(name== documentSnapshot.GetValue<string>("Name"))
                {
                    CoordinatesModel Coordinates = new CoordinatesModel
                    {
                        Longitude = documentSnapshot.GetValue<string>("Longitude"),
                        Latitude = documentSnapshot.GetValue<string>("Latitude"),
                        Name = documentSnapshot.GetValue<string>("Name"),
                        Position = documentSnapshot.GetValue<int>("pos"),

                        // Map more properties as needed
                    };
                    return View(Coordinates);
                }
 
               
            }
            return View();
        }

        // POST: CoordinatesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id)
        {
            string Name = TempData["Name"] as string;
            string uid = TempData["id"] as string;
            string parentCollection = "Trails";
            string parentDocumentId = uid;
           
            System.Diagnostics.Debug.WriteLine($"EDIT POST CALLED: {Name}");
            // Specify the new collection name
            string newCollectionName = "Cordinates";
            
            try
            {
                var trailCollection = _db.Collection(parentCollection);

                var trailDocument = trailCollection.Document(parentDocumentId);

                // Access the child collection "coordinates"
                var coordinatesCollection = trailDocument.Collection(newCollectionName);

                // Update document data inside the "coordinates" collection
                //var coordinateDocument = coordinatesCollection.Document(docUID);
                //await coordinateDocument.UpdateAsync(new Dictionary<string, object>
                //{
                //    { "Longitude" , coordinates.Longitude },
                //    { "Latitude" , coordinates.Longitude },
                //    { "Name" , coordinates.Longitude },
                //    { "Position" , coordinates.Longitude },

                //});
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
