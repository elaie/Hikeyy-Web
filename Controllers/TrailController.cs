using Firebase.Storage;
using Hikeyy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> UploadImages(List<IFormFile> files)
        {
            var firebaseStorage = new FirebaseStorage("<firebase-storage-bucket>");
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var stream = file.OpenReadStream();

                    var imageUrl = await firebaseStorage
                        .Child("images")
                        .Child(fileName)
                        .PutAsync(stream);

                    imageUrls.Add(imageUrl);
                }
            }

            // Process the image URLs or save them in your database

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(TrailModel trail)
        {
            System.Diagnostics.Debug.WriteLine("CREATE TASK METHOD CALLED");
            if (ModelState.IsValid)
            {
                List<string> selectedOptions = trail.BestMonths;
               
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
