using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

public static class FirebaseInitializer
{
    private static bool _initialized = false;
    public static void Initialize()
    {
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
}
