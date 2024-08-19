using DataStoreInsertApp.DataAccess;
using DataStoreInsertApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DataStoreInsertApp.Models;
using DataStoreInsertApp.DataAccess;
using Microsoft.Extensions.Configuration;

namespace DataStoreInsertApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PersonDAL _personDAL;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            string dbConnectionString = configuration["azuresqldbConnection"];
            _personDAL = new PersonDAL(configuration);
            _logger = logger;
        }

        public IActionResult Index()
        {
            var persons = _personDAL.GetAllPersons(); // Fetch all persons from the database
            return View(persons);
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
