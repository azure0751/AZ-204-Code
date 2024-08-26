using Microsoft.AspNetCore.Mvc;

namespace RedisCacheBlobReaderDemo.Controllers
{
    public class DemoConfigController : Controller
    {
        private readonly IConfiguration _configuration;

        public DemoConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Check for necessary configuration settings
            var missingConfigs = new List<string>();

            var blobConnectionString = _configuration["AzureBlobStorage:ConnectionString"];
            var containername = _configuration["AzureBlobStorage:ContainerName"];
            var blobname = _configuration["AzureBlobStorage:BlobName"];
            var redisConnectionString = _configuration["RedisCache:ConnectionString"];

            if (string.IsNullOrEmpty(blobConnectionString))
            {
                missingConfigs.Add("Azure Blob Storage connection string [\"AzureBlobStorage:ConnectionString\"] is missing.");
            }

            if (string.IsNullOrEmpty(blobConnectionString))
            {
                missingConfigs.Add("Azure Blob container name [\"AzureBlobStorage:ContainerName\"] configuration is missing.");
            }

            if (string.IsNullOrEmpty(blobConnectionString))
            {
                missingConfigs.Add("Azure Blob name [\"AzureBlobStorage:BlobName\"] configuration is missing.");
            }

            if (string.IsNullOrEmpty(redisConnectionString))
            {
                missingConfigs.Add("Redis Cache connection string [\"RedisCache:ConnectionString\"] is missing.");
            }

            // Pass the missing configurations to the view
            ViewBag.MissingConfigs = missingConfigs;

            return View();
        }
    }
}