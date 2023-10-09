using InstrumentService.Models;
using InstrumentService_DataAccess;
using InstrumentService_DataAccess.Repository.IRepository;
using InstrumentService_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentService.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ApplicationTypeController : Controller
    {
        private readonly IApplicationTypeRepository _appTypeRep;
        public ApplicationTypeController(IApplicationTypeRepository appTypeRep)
        {
            _appTypeRep = appTypeRep;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<ApplicationType> objlist = await _appTypeRep.GetAllAsync();
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
            _appTypeRep.Add(obj);
            _appTypeRep.Save();
            return RedirectToAction("Index");
        }
        //GET - Edit
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var obj = await _appTypeRep.FindAsync(id.GetValueOrDefault());
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
                _appTypeRep.UpdateAsync(obj);
                _appTypeRep.Save();
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
            var obj = await _appTypeRep.FindAsync(id.GetValueOrDefault());
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
            var obj = await _appTypeRep.FindAsync(id.GetValueOrDefault());
            if (obj == null)
            {
                return NotFound();
            }

            _appTypeRep.Remove(obj);
            _appTypeRep.Save();
            return RedirectToAction("Index");
        }
    }
}
