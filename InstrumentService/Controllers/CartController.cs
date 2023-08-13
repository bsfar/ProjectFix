﻿using InstrumentService.Data;
using InstrumentService.Models;
using InstrumentService.Models.ViewModels;
using InstrumentService.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Security.Claims;
using System.Text;

namespace InstrumentService.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ProductUserCartVM ProductUserCartVM { get; set; }

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            //List<int> prodInCart = shoppingCarts.Select(x => x.ProductId).ToList();
            //IEnumerable<Product> prodList = _db.Product.Where(x => prodInCart.Contains(x.Id));
            IEnumerable<Product> prodList = _db.Product.Where(x => shoppingCarts.Select(x => x.ProductId).Contains(x.Id));


            return View(prodList);
        }

        //методы IndexPost и Summary выполняют разные задачи. Первый метод IndexPost выполняет обработку данных после отправки
        //формы, а второй метод Summary отображает сводку информации.
        //Хотя теоретически можно было бы объединить эти два метода в один, однако это может привести к усложнению кода и нарушению принципа разделения ответственности
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            //в claimsIdentity записываю утверждения о пользователе из http контекста
            ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            //в claim делаю поиск по утверждению, что тип должен был NameIdentifier(Id индефикатор)
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            //List<int> prodInCart = shoppingCarts.Select(x => x.ProductId).ToList();
            //IEnumerable<Product> prodList = _db.Product.Where(x => prodInCart.Contains(x.Id));
            IEnumerable<Product> prodList = _db.Product.Where(x => shoppingCarts.Select(x => x.ProductId).Contains(x.Id));

            ProductUserCartVM = new ProductUserCartVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(x => x.Id == claim.Value),
                ProductList = prodList.ToList()
            };
            return View(ProductUserCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserCartVM productUserCartVM)
        {
            var PastToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "Inquiry.html";

            var subject = "New Inquiry";
            string HtmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(PastToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach (var prop in ProductUserCartVM.ProductList)
            {
                productListSB.Append($" - Имя: {prop.Name} <span style = 'font-size: 14px;'> (ID:{prop.Id})</span><br />");
            }
            string messageBody = string.Format(HtmlBody,
                ProductUserCartVM.ApplicationUser.FullName,
                ProductUserCartVM.ApplicationUser.Email,
                ProductUserCartVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

                return RedirectToAction(nameof(InquiryConfirmation));
        }
        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();

            return View();
        }
        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                //session exsits
                shoppingCarts = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);
            }
            shoppingCarts.Remove(shoppingCarts.FirstOrDefault(x => x.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCarts);

            return RedirectToAction(nameof(Index));
        }
    }
}
