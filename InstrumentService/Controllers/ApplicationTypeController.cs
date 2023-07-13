using InstrumentService.Data;
using InstrumentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentService.Controllers
{
    public class ApplicationTypeController : Controller
    {
        private readonly ApplicationDbContext _dp;
        public ApplicationTypeController(ApplicationDbContext dp)
        {
            _dp = dp;
        }

        public IActionResult Index()
        {
            IEnumerable<ApplicationType> objlist = _dp.ApplicationTypes;
            return View(objlist);
        }

        //GET - Create
        public IActionResult Create()
        {
            return View();
        }
        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ApplicationType obj)
        {
            _dp.ApplicationTypes.Add(obj);
            _dp.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
