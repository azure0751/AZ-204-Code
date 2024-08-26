using Azure.Storage.Blobs;

namespace RedisCacheBlobReaderDemo.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly string _blobName;

        public BlobService()
        {
        }

        public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
            _containerName = configuration["AzureStorage:ContainerName"];
            _blobName = configuration["AzureStorage:BlobName"];
        }

        public BlobService(string connectionString, string containerName, string blobName)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName;
            _blobName = blobName;
        }

        public async Task<string> ReadBlobContentAsync()
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(_blobName);
            using (var stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task UpdateBlobContentAsync(string content)
        {
            // var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(_blobName);

            using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }
        }
    }
}