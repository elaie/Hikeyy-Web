using Firebase.Auth;
using Google.Cloud.Firestore;
using Hikeyy.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Hikeyy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private FirestoreDb _db;
        FirebaseAuthProvider auth;
        public HomeController(ILogger<HomeController> logger)
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig("AIzaSyC_qh0TxNX0RXDuPfB5tfOO86GnAx3dY6Q"));
            System.Diagnostics.Debug.WriteLine("COORDINATES KO CONSTRUCTOR CALLED HAI");
            var credentialsPath = "hikeyyy-firebase-adminsdk-oj70g-ca14e13827.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialsPath);

            // Initialize Firestore client
            _db = FirestoreDb.Create("hikeyyy");

        }

        [HttpGet]
        public async Task<JsonResult> GetLocations()
        {
            var parentCollectionName = "Groups";
            var childCollectionName = "Locations";

            var parentQuery = _db.Collection(parentCollectionName);
            var parentSnapshot = await parentQuery.GetSnapshotAsync();
            List<Dictionary<string, object>> locations = new List<Dictionary<string, object>>();
            
            foreach (var parentDocument in parentSnapshot.Documents)
            {
                var childQuery = parentDocument.Reference.Collection(childCollectionName);
                var childSnapshot = await childQuery.GetSnapshotAsync();
                System.Diagnostics.Debug.WriteLine("INSIDE PARENT");
                foreach (var childDocument in childSnapshot.Documents)
                {
                    Dictionary<string, object> locationData = childDocument.ToDictionary();
                    locations.Add(locationData);
                    System.Diagnostics.Debug.WriteLine("INSIDE CHILD" + locationData["latitude"]);
                }
            }
            return Json(locations);
        }
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("_UserToken");

            if (token != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("SignIn");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("_UserToken");
            return RedirectToAction("SignIn");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Registration(LoginModel loginModel)
        {
            try
            {
                //create the user
                await auth.CreateUserWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                //log in the new user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                string token = fbAuthLink.FirebaseToken;
                //saving the token in a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel loginModel)
        {
            // Create a new product
           /* var product = new Product
            {
                Name = "New Product",
                Price = 9.99
            };

            string productId = await _productRepository.CreateAsync(product);*/



            try
            {
                
                //log in an existing user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                string token = fbAuthLink.FirebaseToken;
                //save the token to a session variable
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index");
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();
        }
    }
}