using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using AzureStorageAccountDemo.Models;
using Azure.Storage.Sas;

namespace AzureStorageAccountDemo.Services
{
    public class BlobService
    {
        private readonly string _connectionString;

        public BlobService(IConfiguration configuration)
        {
            // _connectionString = configuration.GetSection("AzureStorage:ConnectionString").Value;
            _connectionString = configuration["AzureStorage:ConnectionString"];
            string debugdoing = "";
        }

        public async Task<string> UploadFileAsync(FileUploadModel model)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient("mycontainer");

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            var blobClient = containerClient.GetBlobClient(model.FileName);

            using (var stream = model.File.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = model.File.ContentType });
            }

            return blobClient.Uri.ToString();
        }

        public async Task<List<string>> ListContainersAsync()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containers = new List<string>();

            await foreach (var container in blobServiceClient.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }

            return containers;
        }


        public async Task<List<string>> ListBlobsAsync(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = new List<string>();

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(blobItem.Name);
            }

            return blobs;
        }
        public async Task CreateContainerAsync(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainerAsync(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.DeleteIfExistsAsync();
        }

        public string GenerateSasToken(string containerName, string blobName, int expiryHours = 1)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (blobClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(expiryHours)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                var sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri.ToString();
            }
            else
            {
                throw new InvalidOperationException("SAS token cannot be generated. Please ensure the storage account supports generating SAS tokens.");
            }
        }
    }
}