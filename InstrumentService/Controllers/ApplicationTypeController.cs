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
        //GET - Edit
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = _dp.ApplicationTypes.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }
        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApplicationType obj)
        {
            if (ModelState.IsValid)
            {
                _dp.ApplicationTypes.Update(obj);
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
            var obj = _dp.ApplicationTypes.Find(id);
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
            var obj = _dp.ApplicationTypes.Find(id);
            if (obj == null)
            {
                return NotFound();
            }

            _dp.ApplicationTypes.Remove(obj);
            _dp.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
