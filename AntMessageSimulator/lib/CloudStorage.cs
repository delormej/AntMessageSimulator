using System;
// using Microsoft.WindowsAzure.Storage;
// using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace AntMessageSimulator
{
    public class CloudStorage
    {
        // CloudBlobContainer container;

        public CloudStorage()
        {
            // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(GetConnectionString());
            // CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // container = blobClient.GetContainerReference("rides");
        }

        public void Upload(string path)
        {
            try
            {
                // string name = Path.GetFileName(path);
                // CloudBlockBlob blob = container.GetBlockBlobReference(name);
                // blob.DeleteIfExists();
                // blob.UploadFromFile(path);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to upload to Azure.", e);
            }
        }

        private string GetConnectionString()
        {
            string connectionString = Environment.GetEnvironmentVariable("CLOUD_STORAGE_ACCOUNT");
            if (string.IsNullOrEmpty(connectionString))
                throw new ApplicationException("Must set CLOUD_STORAGE_ACCOUNT environment variable.");
            return connectionString;
        }
    }
}
