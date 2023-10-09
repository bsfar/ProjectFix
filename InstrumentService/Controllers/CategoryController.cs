using InstrumentService.Models;
using InstrumentService_DataAccess;
using InstrumentService_DataAccess.Repository.IRepository;
using InstrumentService_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentService.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _catRep;
        public CategoryController(ICategoryRepository categoryRepository)
        {

            _catRep = categoryRepository;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<Category> objlist = await _catRep.GetAllAsync();
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
                _catRep.Add(obj);
                _catRep.Save();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //GET - Edit
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = await _catRep.FindAsync(id.GetValueOrDefault());
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
                _catRep.UpdateAsync(obj);
                _catRep.Save();
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        //GET - Delete
        [HttpGet]
        public async Task<IActionResult> DeleteAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = await _catRep.FindAsync(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        //POST - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePostAsync(int? id)
        {
            var obj = await _catRep.FindAsync(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            _catRep.Remove(obj);
            _catRep.Save();
            return RedirectToAction("Index");
        }
    }
}
