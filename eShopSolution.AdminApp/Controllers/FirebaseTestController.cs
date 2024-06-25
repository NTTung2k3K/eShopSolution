using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace eShopSolution.AdminApp.Controllers
{
    public class FirebaseTestController : Controller
    {
        private static string ApiKey = "AIzaSyCWAHAugxsHIs_JStlv5JGyuFHNXi9kX9Q";
        private static string Bucket = "testfirebase-e3ab2.appspot.com";
        private static string AuthEmail = "tester@gmail.com";
        private static string AuthPassword = "tester123";
        private IWebHostEnvironment _env;

        public FirebaseTestController(IWebHostEnvironment web)
        {
            _env = web;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /* 
         *  Save image at firebase and server
         * 
         * 
         * [HttpPost]

         public async Task<IActionResult> Index([FromForm] IFormFile file)
         {
             if (file!=null  && file.Length > 0)
             {
                 string path = Path.Combine(_env.WebRootPath, "Content\\images");
                 if(!Directory.Exists(path))
                 {
                     Directory.CreateDirectory(path);
                 }
                 string filePath = Path.Combine(path, file.FileName);


                  using (var stream = new FileStream(filePath, FileMode.Create))
                 {
                     await file.CopyToAsync(stream);
                 }
                 using (var stream = new FileStream(filePath, FileMode.Open))
                 {
                     string url = await Upload(stream, file.FileName);
                     ViewBag.firebaseUrl = url;
                 }

                 return View();
             }
             return View();
         }
        
         
        private async Task<string> Upload(FileStream stream, string fileName)
        {

            // of course you can login using other method, not just email+password
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            // you can use CancellationTokenSource to cancel the upload midway
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("images")
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // cancel the upload
            // cancellation.Cancel();

            try
            {
                string link = await task;
                return link;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return ex.Message.ToString();
            }
        }
         
         
         
         */


        [HttpPost]
        public async Task<IActionResult> Index([FromForm] IFormFile file)
        {
            string link = await Upload(file);
            return View();
        }



        private async Task<string> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string firebaseUrl = "";
                // Use a MemoryStream to avoid saving the file locally
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0; // Reset the stream position to the beginning

                    firebaseUrl = await UploadToFirebase(memoryStream, file.FileName); // Get the download URL
                }
                return firebaseUrl;

            }
            return "";
        }


        private async Task<string> UploadToFirebase(Stream stream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("images")
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            try
            {
                string link = await task;
                return link;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception was thrown: {0}", ex);
                return null;
            }
        }

    }
}
