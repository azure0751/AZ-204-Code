using AzureStorageAccountDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureStorageAccountDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration["AzureStorage:ConnectionString"];
            bool isConfigMissing = string.IsNullOrEmpty(connectionString);
            if (string.IsNullOrEmpty(connectionString))
            {
                ViewBag.WarningMessage = "Warning: The AzureStorage__ConnectionString  is missing! kindly set it in environment variable or app settings.json ";
                ViewBag.IsConfigMissing = isConfigMissing;

            }

           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
