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
            if (ModelState.IsValid)
            {
                _dp.Category.Add(obj);
                _dp.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _dp.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _dp.Category.Update(obj);
                _dp.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //GET - Delete
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _dp.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _dp.Category.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            
                _dp.Category.Remove(obj);
                _dp.SaveChanges();
                return RedirectToAction("Index");
        }
    }
}
