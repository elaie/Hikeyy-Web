using Hikeyy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using Google.Cloud.Storage.V1;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
namespace Hikeyy.Controllers
{
    public class TrailController : Controller
    {

        // GET: TrailController
        private readonly FirestoreRepository<TrailModel> _trailRepository;
        public TrailController()
        {
            FirebaseInitializer.Initialize();
            _trailRepository = new FirestoreRepository<TrailModel>("hikeyyy", "testtrail");
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
            List<TrailModel> allProducts = await _trailRepository.GetAllAsync();
            return View(allProducts);
        }

        // GET: TrailController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TrailController/Create
        public ActionResult Create()
        {
            TrailModel model = new TrailModel();
            model.BestMonths = new List<string>();
            return View(model);
        }

        private async Task<string> UploadImageToStorage(IFormFile photo, string fileName)
        {
            var storage = StorageClient.Create();
            System.Diagnostics.Debug.WriteLine("UploadImageToStorageCalled");
            string bucketName = "hikeyyy.appspot.com";
            string folderName = "images";
            var objectName = "";
            string mimeType = photo.ContentType;
            using (var stream = photo.OpenReadStream())
            {
                objectName = $"{folderName}/{fileName}";
                
                await storage.UploadObjectAsync(bucketName, objectName, mimeType, stream);
            }

            string imageUrl = $"https://storage.googleapis.com/{bucketName}/{objectName}";

            return imageUrl;
        }

        [HttpPost]
        public async Task<IActionResult> Create(TrailModel trail, List<IFormFile> photos)
        {
            System.Diagnostics.Debug.WriteLine("CREATE TASK METHOD CALLED");
            if (ModelState.IsValid)
            {
                if (photos != null && photos.Count > 0)
                {
                    trail.PhotoURLs = new List<string>();

                    foreach (var photo in photos)
                    {
                        // Generate a unique filename for each uploaded image
                        string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                        // Upload the image to Firebase Storage or any other storage provider
                        // and get the URL of the uploaded image
                        string imageUrl = await UploadImageToStorage(photo, fileName);

                        // Add the image URL to the trail model
                        trail.PhotoURLs.Add(imageUrl);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("PHOTOS ARE EMPTY");
                }
                List<string> selectedOptions = trail.BestMonths;
                System.Diagnostics.Debug.WriteLine("MODEL IS VALID");
                System.Diagnostics.Debug.WriteLine(trail.Name);
                await _trailRepository.CreateAsync(trail);
                return RedirectToAction("Index");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("MODELSTATE IS INVALID");
                System.Diagnostics.Debug.WriteLine(trail.Name);
            }

            return View(trail);
        }
        // POST: TrailController/Create
      

        // GET: TrailController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TrailController/Edit/5
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

        // GET: TrailController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TrailController/Delete/5
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
