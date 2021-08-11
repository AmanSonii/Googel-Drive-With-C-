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
            try
            {
                UserCredential credential;

                string UserName = "amansoni887127@gmail.com";                

                LogMessage("Service Started");

                using (var stream = new FileStream("client-secret.json", FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                       new[] { DriveService.Scope.Drive },
                        UserName,
                        CancellationToken.None,
                        new FileDataStore("Drive.Api.Auth.Store")).Result;
                }

                LogMessage("Authentication Done");

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential
                });

                Console.WriteLine("Enter filename to upload on google drive");

                var fileName = Console.ReadLine();

                LogMessage($"File Name is {fileName}");

                uploadFileOnGoogleDrive(service, fileName);

                Console.WriteLine("Hello World!");
            }
            catch(Exception e)
            {
                LogMessage(e.Message);
            }
            
        }

        public static void uploadFileOnGoogleDrive(DriveService _service, string _uploadFile)
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = System.IO.Path.GetFileName(_uploadFile + DateTime.Now);
                var mimeType = GetMimeType(_uploadFile);
                body.MimeType = mimeType;
                byte[] byteArray = File.ReadAllBytes(_uploadFile);
                var stream = new MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, mimeType);
                    request.SupportsTeamDrives = true;
                     request.Upload();
                    var response =  request.ResponseBody;
                    Console.WriteLine("Upload Completed!");
                    LogMessage($"Upload Completed for {_uploadFile}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    LogMessage(e.Message);
                }
            }
            else
            {
                Console.WriteLine("File Doesn't exists!");
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

        private static void LogMessage(string message)
        {
           
                var folderName = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                
                var fileName = string.Format("{0}.txt", DateTime.Now.ToString("MMddyyyy"));
                fileName = "Logs" + "_" + fileName;

                if (!Directory.Exists(folderName))
                    Directory.CreateDirectory(folderName);

                var fs = new FileStream(@"D:\Aman Projects\" + fileName, FileMode.Append);

                var str = new StreamWriter(fs);
                str.AutoFlush = true;
                str.WriteLine("Time: " + DateTime.Now);
                str.WriteLine(message);
                //str.WriteLine(Environment.NewLine);
                str.WriteLine("========================================================================================");

                str.Flush();
                str.Close();
                fs.Close();
            
            
        }

    }
}
