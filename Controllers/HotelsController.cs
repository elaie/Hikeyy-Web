using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Firebase.Auth;
using Google.Cloud.Firestore;
using Hikeyy.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Hikeyy.Controllers
{
    public class HotelsController : Controller
    {
        // GET: HotelsController
        private static string ApiKey = "AIzaSyC_qh0TxNX0RXDuPfB5tfOO86GnAx3dY6Q";
        private static string Bucket = "hikeyyy.appspot.com";
        private static string AuthEmail = "admin@gmail.com";
        private static string AuthPassword = "admin123";
        // GET: TrailController
        private readonly FirestoreRepository<HotelsModel> _trailRepository;
        public HotelsController()
        {

            FirebaseInitializer.Initialize();
            _trailRepository = new FirestoreRepository<HotelsModel>("hikeyyy", "Hotels");
            if (FirebaseApp.DefaultInstance == null)
            {
                System.Diagnostics.Debug.WriteLine("DefaultInstance is NULL");
                string path = "hikeyyy-firebase-adminsdk-oj70g-ca14e13827.json"; // Replace with the actual path to your service account key JSON file
                string projectId = "hikeyyy"; // Replace with your actual Firebase project ID

                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                    ProjectId = projectId
                });
            }
        }

        public async Task<IActionResult> Index()
        {
            List<HotelsModel> hotels = await _trailRepository.GetAllAsync();
            return View(hotels);
        }

        // GET: HotelsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HotelsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HotelsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: HotelsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HotelsController/Edit/5
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

        // GET: HotelsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HotelsController/Delete/5
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
