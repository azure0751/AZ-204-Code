using Microsoft.AspNetCore.Mvc;
using RedisCacheBlobReaderDemo.Services;
using RedisCacheBlobReaderDemo.Services;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly BlobService _blobService;
    private readonly RedisCacheService _redisCacheService;

    public HomeController(BlobService blobService, RedisCacheService redisCacheService)
    {
        _blobService = blobService;
        _redisCacheService = redisCacheService;
    }

    public async Task<IActionResult> Index()
    {
        const string cacheKey = "blobContent";

        // Check if content is in cache
        var cachedContent = await _redisCacheService.GetCacheAsync(cacheKey);
        if (cachedContent == null)
        {
            // Read content from Azure Blob Storage
            var content = await _blobService.ReadBlobContentAsync();

            // Store content in Redis Cache with a 90-second expiration
            await _redisCacheService.SetCacheAsync(cacheKey, content, TimeSpan.FromSeconds(90));


            // Set content to be displayed
            ViewData["Content"] = content;
        }
        else
        {
            // Use cached content
            ViewData["Content"] = cachedContent;
        }

        return View();
    }

    // Action method to display the edit form with the current blob content
    public async Task<IActionResult> EditBlobContent()
    {
        // Fetch the current content from the blob
        var currentContent = await _blobService.ReadBlobContentAsync();
        return View((object)currentContent);
    }

    // Action method to handle the form submission and update the blob content
    [HttpPost]
    public async Task<IActionResult> EditBlobContent(string content)
    {
        // Update the blob content
        await _blobService.UpdateBlobContentAsync(content);

        // Invalidate and refresh the Redis cache with the new content
        const string cacheKey = "blobContent";
        await _redisCacheService.SetCacheAsync(cacheKey, content, TimeSpan.FromSeconds(90));

        // Redirect back to the Index page
        return RedirectToAction("Index");
    }
}
