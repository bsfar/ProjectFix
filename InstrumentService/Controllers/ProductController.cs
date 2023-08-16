using InstrumentService.Data;
using InstrumentService.Models;
using InstrumentService.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace InstrumentService.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> IndexAsync()
        {
            IEnumerable<Product> products = await _db.Product
            .Include(p => p.Category)
            .Include(p => p.ApplicationType)
            .ToArrayAsync();


            return View(products);
        }

        //GET - UPSERT
        public IActionResult Upsert(int? id)
        {

            //IEnumerable<SelectListItem> CategoryDropDown = _db.Category.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.CategoryDropDown = CategoryDropDown;

            //Product product = new Product();
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Category.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null)
            {
                //this is for create 
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Product.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                return View(productVM);
            }
        }

        //POST - UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM)
        {
            ModelState.Remove("Product.Category");
            ModelState.Remove("Product.ApplicationType");
            if (!ModelState.IsValid)
            {

                Category[] categorys = await _db.Category.ToArrayAsync();
                IEnumerable<SelectListItem> categoryDropDown = categorys.Select(c =>
                    new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    });
                ApplicationType[] applicationTypes = await _db.ApplicationTypes.ToArrayAsync();
                IEnumerable<SelectListItem> AppTypeDropDown = applicationTypes.Select(at =>
                    new SelectListItem
                    {
                        Text = at.Name,
                        Value = at.Id.ToString()
                    });


                productVM.CategorySelectList = categoryDropDown;
                productVM.ApplicationTypeSelectList = AppTypeDropDown;
                return View(productVM);
            }
            else
            {
                //HttpContext представляет текущий HTTP-контекст, а Request - объект, содержащий информацию о текущем HTTP-запросе.
                //Свойство Form используется для доступа к форме, отправленной с запросом. Если запрос содержит файлы (например,
                //загруженные пользователем изображения), то они будут доступны через Form.Files

                var files = HttpContext.Request.Form.Files;
                //webRootPath = "D:\\Projects\\InstrumentService\\InstrumentService\\wwwroot"
                //WebRootPath возвращает путь к корневой папке веб-приложения, где обычно размещаются статические файлы,
                //такие как изображения, таблицы стилей (CSS), скрипты (JavaScript) и другие ресурсы, доступные клиентам.
                string webRootPath = _webHostEnvironment.WebRootPath;

                if (productVM.Product.Id == 0)
                {
                    //creating
                    //путь до wwwroot объединятся с путем по папки images/product
                    string upload = webRootPath + WC.ImagePath;
                    //Guid.NewGuid() - генерит уникальный индефикатор 
                    string fileName = Guid.NewGuid().ToString();
                    //Получается расширение файла из имени первого загруженного файла (files[0].FileName)
                    string extension = Path.GetExtension(files[0].FileName);
                    //Path.Combine(upload, fileName + extension) создает полный путь к файлу

                    //FileMode.Create указывает, что файл будет создан или перезаписан, если он уже существует

                    //Объекты, которые реализуют интерфейс IDisposable, требуют явного освобождения ресурсов,
                    //чтобы избежать утечек памяти и других проблем с управлением ресурсами.
                    //Оператор using позволяет автоматически вызывать метод Dispose() объекта при выходе из блока кода. 
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVM.Product.Image = fileName + extension;
                    _db.Product.Add(productVM.Product);

                }
                else
                {
                    //updating
                    var objFromDb = _db.Product.AsNoTracking().FirstOrDefault(u => u.Id == productVM.Product.Id);

                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        productVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        productVM.Product.Image = objFromDb.Image;
                    }
                    _db.Product.Update(productVM.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
        }


        //GET - Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? product = await _db.Product
                .Include(p => p.Category)
                .Include(p => p.ApplicationType)
                .FirstOrDefaultAsync(p => p.Id == id);
            //product.Category = _db.Category.Find(product.CategoryId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Product.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            string upload = _webHostEnvironment.WebRootPath + WC.ImagePath;

            var oldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Product.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
