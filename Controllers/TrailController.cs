using Hikeyy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.AccessControl;
using Google.Cloud.Storage.V1;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Firebase.Auth;
using Firebase.Storage;
using System.Web;
using System.Reflection.PortableExecutable;

namespace Hikeyy.Controllers
{
    public class TrailController : Controller
    {
        private static string ApiKey = "AIzaSyC_qh0TxNX0RXDuPfB5tfOO86GnAx3dY6Q";
        private static string Bucket = "hikeyyy.appspot.com";
        private static string AuthEmail = "admin@gmail.com";
        private static string AuthPassword = "admin123";
        // GET: TrailController
        private readonly FirestoreRepository<TrailModel> _trailRepository;
        public TrailController()
        {

            FirebaseInitializer.Initialize();
            _trailRepository = new FirestoreRepository<TrailModel>("hikeyyy", "Trails");
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


        public async Task<string> Upload(FileStream stream, string fileName,string name)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child("Trails")
                .Child(name)
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);
            string link = await task;
            System.Diagnostics.Debug.WriteLine("LINKKKKKKKKKKKK" + link);
            return link;
        }
        public async Task<IActionResult> Index()
        {
            List<TrailModel> trails = await _trailRepository.GetAllAsync();
            return View(trails);
        }

        // GET: TrailController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            TrailModel model = await _trailRepository.GetAsync(id);
            return View(model);
        }

        // GET: TrailController/Create
        public ActionResult Create()
        {
            TrailModel model = new TrailModel();
            model.BestMonths = new List<string>();
            return View(model);
        }

        private async Task<string> UploadImageToStorage(IFormFile photo, string fileName,string trailName)
        {
            var storage = StorageClient.Create();
            System.Diagnostics.Debug.WriteLine("UploadImageToStorageCalled");
            string bucketName = "hikeyyy.appspot.com";
            string folderName = "Trails/"+trailName;
            var objectName = "";
            string mimeType = photo.ContentType;
            using (var stream = photo.OpenReadStream())
            {
                objectName = $"{folderName}/{fileName}";
  
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
                FileStream stream;
                if (photos != null && photos.Count > 0)
                {
                    trail.PhotoURLs = new List<string>();

                    foreach (var photo in photos)
                    {
                        // Generate a unique filename for each uploaded image
                        string fileName = Path.GetFileName(photo.FileName);
                        string fileName2 = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                        string filePath = Path.Combine(System.IO.Path.GetFullPath("C:\\Users\\elaie\\Documents\\Hikeyy Web\\Hikeyy"), fileName);
                        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string imagePath = Path.Combine(baseDirectory, "Content/images/", photo.FileName);
                        System.Diagnostics.Debug.WriteLine("IMG PATH"+ fileName2);
                        stream = new FileStream(Path.Combine(filePath), FileMode.Open);

                        
                        //string imageUrl = await UploadImageToStorage(photo, fileName, trail.Name);
                        
                        string imageUrl = await Upload(stream, photo.FileName,trail.Name);

                        System.Diagnostics.Debug.WriteLine("IMAGE URL IN VIEW " + imageUrl);

                        // Upload the image to Firebase Storage or any other storage provider
                        // and get the URL of the uploaded image
                        

                        //// Add the image URL to the trail model
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
        public async Task<IActionResult> Edit(string id)
        {
            TrailModel model = new TrailModel();
            model = await _trailRepository.GetAsync(id);
            System.Diagnostics.Debug.WriteLine("ID HAI HAMRO EDIT KO" + id);
            System.Diagnostics.Debug.WriteLine("MODEL KO NAME" + model.Name);
            return View(model);
        }

        // POST: TrailController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TrailModel trail)
        {
            System.Diagnostics.Debug.WriteLine("ID HAI HAMRO EDIT KO post request ma hai" + id);
            
                System.Diagnostics.Debug.WriteLine("MODEL IS VALID INSIDE EDIT NAME: "+trail.Name);
                await _trailRepository.UpdateAsync(id,trail);
                return RedirectToAction("Index");
           
        }

        // GET: TrailController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            TrailModel model = await _trailRepository.GetAsync(id);
            return View(model);
        }

        // POST: TrailController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, IFormCollection collection)
        {
            System.Diagnostics.Debug.WriteLine("UID TO BE DELETED" + id);
            await _trailRepository.DeleteAsync(id);

            return RedirectToAction("Index");

        }
    }
}
