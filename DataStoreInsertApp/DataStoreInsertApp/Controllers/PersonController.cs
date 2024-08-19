using DataStoreInsertApp.DataAccess;
using DataStoreInsertApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DataStoreInsertApp.Controllers
{
    public class PersonController : Controller
    {
        private readonly PersonDAL _personDAL;

        public PersonController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _personDAL = new PersonDAL(configuration);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Person person)
        {
            if (ModelState.IsValid)
            {
                _personDAL.InsertPerson(person);
                return RedirectToAction("Index", "Home");
            }
            return View(person);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
