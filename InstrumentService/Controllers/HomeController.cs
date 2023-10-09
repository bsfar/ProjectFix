using InstrumentService.Models;
using InstrumentService.Models.ViewModels;
using InstrumentService_DataAccess.Repository.IRepository;
using InstrumentService_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InstrumentService_DataAccess.Migrations
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepository productRepo, ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _logger = logger;
            _categoryRepo = categoryRepo;
        }
        
        public async Task<IActionResult> IndexAsync()
        {
            HomeVM homeVM = new HomeVM()
            {
                Products = await _productRepo.GetAllAsync(includeProperties: "Category,ApplicationType"),
                Categories = await _categoryRepo.GetAllAsync(),
            };
            return View(homeVM);
        }
        public async Task<IActionResult> DetailsAsync(int id)
        {
            //получаем данные о сессии
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }


            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = await _productRepo.FirstOrDefaultAsync(u => u.Id == id, includeProperties: "Category,ApplicationType"),
                
                ExistsInCart = false
            };
            //Проверка есть ли товар в сессии
            foreach (var item in shoppingCartsList)
            {
                if (item.ProductId == id)
                {
                    DetailsVM.ExistsInCart = true;
                }
            }
            return View(DetailsVM);
        }
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCartsList.Add(new ShoppingCart { ProductId = id });
            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);
            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                shoppingCartsList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            var itemToRemove = shoppingCartsList.SingleOrDefault(r => r.ProductId == id);
            if(itemToRemove != null)
            {
                shoppingCartsList.Remove(itemToRemove);
            }
            HttpContext.Session.Set(WC.SessionCart, shoppingCartsList);
            return RedirectToAction("Index");
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