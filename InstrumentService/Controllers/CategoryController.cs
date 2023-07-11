using InstrumentService.Data;
using InstrumentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentService.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _dp;
        public CategoryController(ApplicationDbContext dp)
        {
            _dp = dp;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objlist = _dp.Category;
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
        public IActionResult Create(Category obj)
        {
            _dp.Category.Add(obj);
            _dp.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
