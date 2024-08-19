using AzureStorageAccountDemo.Models;
using AzureStorageAccountDemo.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageAccountDemo.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly BlobService _blobService;

        public FileUploadController(BlobService blobService)
        {
            _blobService = blobService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(FileUploadModel model)
        {
            if (model.File != null && model.File.Length > 0)
            {
                model.FileName = model.File.FileName;
                var result = await _blobService.UploadFileAsync(model);
                ViewBag.Message = $"File uploaded successfully: {result}";
            }
            else
            {
                ViewBag.Message = "Please select a file to upload.";
            }

            return View("Index");
        }

        public async Task<IActionResult> ListBlobs()
        {
            var containers = await _blobService.ListContainersAsync();
            var blobs = new Dictionary<string, List<string>>();

            foreach (var container in containers)
            {
                var blobList = await _blobService.ListBlobsAsync(container);
                blobs.Add(container, blobList);
            }

            var viewModel = new BlobListViewModel
            {
                Containers = containers,
                Blobs = blobs
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ContainerOperations()
        {
            var containers = await _blobService.ListContainersAsync();
            return View(containers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            if (!string.IsNullOrWhiteSpace(containerName))
            {
                await _blobService.CreateContainerAsync(containerName);
                ViewBag.Message = $"Container '{containerName}' created successfully.";
            }
            else
            {
                ViewBag.Message = "Container name cannot be empty.";
            }

            return RedirectToAction("ContainerOperations");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            if (!string.IsNullOrWhiteSpace(containerName))
            {
                await _blobService.DeleteContainerAsync(containerName);
                ViewBag.Message = $"Container '{containerName}' deleted successfully.";
            }
            else
            {
                ViewBag.Message = "Container name cannot be empty.";
            }

            return RedirectToAction("ContainerOperations");
        }

        public async Task<IActionResult> GenerateSasToken()
        {
            var containers = await _blobService.ListContainersAsync();
            var blobs = new Dictionary<string, List<string>>();

            foreach (var container in containers)
            {
                var blobList = await _blobService.ListBlobsAsync(container);
                blobs.Add(container, blobList);
            }

            var viewModel = new BlobListViewModel
            {
                Containers = containers,
                Blobs = blobs
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSasToken(string containerName, string blobName, int expiryHours = 1)
        {
            var containers = await _blobService.ListContainersAsync();
            var blobs = new Dictionary<string, List<string>>();

            foreach (var container in containers)
            {
                var blobList = await _blobService.ListBlobsAsync(container);
                blobs.Add(container, blobList);
            }

            var viewModel = new BlobListViewModel
            {
                Containers = containers,
                Blobs = blobs
            };

            try
            {
                var sasTokenUrl = _blobService.GenerateSasToken(containerName, blobName, expiryHours);
                ViewBag.Message = $"SAS Token URL: <a href='{sasTokenUrl}' target='_blank'>{sasTokenUrl}</a>";
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Error generating SAS token: {ex.Message}";
            }

            return View(viewModel);
        }


    }
}