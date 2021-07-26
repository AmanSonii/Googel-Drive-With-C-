using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace Googel_Drive
{
    class Program
    {
        static void Main(string[] args)
        {
            string credPath = @"D:\StorePath\";
            UserCredential credential;
            string[] scopes = {
            "https://www.googleapis.com/auth/photoslibrary.sharing",
            "https://www.googleapis.com/auth/photoslibrary.readonly"
         };
            string UserName = "amansoni887127@gmail.com";


            using (var stream = new FileStream("client-secret.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                   new[] { DriveService.Scope.Drive },
                    UserName,
                    CancellationToken.None,
                    new FileDataStore("Drive.Api.Auth.Store")).Result;
            }
            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential
            });

            uploadFile(service, @"D:\Aman Projects\desk\columndesign.html", "");

            Console.WriteLine("Hello World!");
        }

        public static Google.Apis.Drive.v3.Data.File uploadFile(DriveService _service, string _uploadFile, string _parent, string _descrp = "Uploaded with .NET!")
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = System.IO.Path.GetFileName(_uploadFile);
                body.Description = _descrp;                
                body.MimeType = GetMimeType(_uploadFile);
                // body.Parents = new List<string> { _parent };// UN comment if you want to upload to a folder(ID of parent folder need to be send as paramter in above method)
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, GetMimeType(_uploadFile));
                    request.SupportsTeamDrives = true;
                    // You can bind event handler with progress changed event and response recieved(completed event)

                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    // MessageBox.Show(e.Message, "Error Occured");
                    return null;
                }
            }
            else
            {
                // MessageBox.Show("The file does not exist.", "404");
                return null;
            }
        }

        private static string GetMimeType(string fileName) { 
            string mimeType = "application/unknown"; 
            string ext = System.IO.Path.GetExtension(fileName).ToLower(); 
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext); 
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString(); 
            
            return mimeType; 
        }

    }
}
