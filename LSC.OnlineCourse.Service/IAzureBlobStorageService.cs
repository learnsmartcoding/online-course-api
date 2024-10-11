using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC.OnlineCourse.Service
{
    public interface IAzureBlobStorageService
    {
        Task<string> UploadAsync(byte[] fileData, string fileName, string containerName = "");
    }

    public class AzureBlobStorageService : IAzureBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "images";

        public AzureBlobStorageService(IConfiguration configuration)
        {
            // Fetch the connection string directly from configuration
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadAsync(byte[] fileData, string fileName, string containerName = "")
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(string.IsNullOrEmpty(containerName) ? _containerName : containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = new MemoryStream(fileData))
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }
    }



}
