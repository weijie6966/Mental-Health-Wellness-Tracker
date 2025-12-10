using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mental_Health_Wellness_Tracker.Services
{
    public class FirebaseStorageService
    {
        private const string StorageBucket = "mental-health-wellness-tracker.firebasestorage.app";

        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string authToken)
        {
            try
            {
                // Create a FirebaseStorage instance
                var storage = new FirebaseStorage(
                    StorageBucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(authToken),
                        ThrowOnCancel = true
                    });

                // Upload logic:
                // 1. Child("diary_images"): Creates a folder
                // 2. Child(fileName): Filename
                // 3. PutAsync(imageStream): Starts the upload
                var downloadUrl = await storage
                    .Child("diary_images")
                    .Child(fileName)
                    .PutAsync(imageStream);

                // Returns to the download link in the cloud (https://...)
                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image upload failed: {ex.Message}");
                return null;
            }
        }
    }
}
