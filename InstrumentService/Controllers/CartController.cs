using InstrumentService.Models;
using InstrumentService.Models.ViewModels;
using InstrumentService_DataAccess;
using InstrumentService_DataAccess.Repository.IRepository;
using InstrumentService_Models;
using InstrumentService_Utility;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _appUserRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInquiryDetailRepository _inquiryDetailRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;


        [BindProperty]
        public ProductUserCartVM ProductUserCartVM { get; set; }

        public CartController( IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IInquiryHeaderRepository inquiryHeaderRepo,
            IInquiryDetailRepository inquiryDetailRepo, IProductRepository productRepo, IApplicationUserRepository appUserRepo)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;

            _inquiryHeaderRepo = inquiryHeaderRepo;
            _productRepo = productRepo;
            _appUserRepo = appUserRepo;
            _inquiryDetailRepo = inquiryDetailRepo;

        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
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
            IEnumerable<Product> prodList = await _productRepo.GetAllAsync(x => shoppingCarts.Select(x => x.ProductId).Contains(x.Id));

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
            return RedirectToAction("Summary");
        }
        public async Task<IActionResult> SummaryAsync()
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
            IEnumerable<Product> prodList = await _productRepo.GetAllAsync(x => shoppingCarts.Select(x => x.ProductId).Contains(x.Id));

            ProductUserCartVM = new ProductUserCartVM()
            {
                ApplicationUser = await _appUserRepo.FirstOrDefaultAsync(x => x.Id == claim.Value),
                ProductList = prodList.ToList()
            };
            return View(ProductUserCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserCartVM productUserCartVM)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //путь на разных системах иметь разные символы, поэтому используем Path.DirectorySeparatorChar
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
            //создаем строчки товаров, которые находятся в контексте
            foreach (var prop in ProductUserCartVM.ProductList)
            {
                productListSB.Append($" - Имя: {prop.Name} <span style = 'font-size: 14px;'> (ID:{prop.Id})</span><br />");
            }
            //string.Format опирается на плейсхолдеры и по сути вставляет в них нужные строки
            string messageBody = string.Format(HtmlBody,
                ProductUserCartVM.ApplicationUser.FullName,
                ProductUserCartVM.ApplicationUser.Email,
                ProductUserCartVM.ApplicationUser.PhoneNumber,
                productListSB.ToString());

            await _emailSender.SendEmailAsync(WC.EmailAdmin, subject, messageBody);

            InquiryHeader inquiryHeader = new InquiryHeader()
            {
                ApplicationUserId = claim.Value,
                FullName = ProductUserCartVM.ApplicationUser.FullName,
                Email = ProductUserCartVM.ApplicationUser.Email,
                PhoneNumber = ProductUserCartVM.ApplicationUser.PhoneNumber,
                InquiryDate = DateTime.UtcNow
            };
            _inquiryHeaderRepo.Add(inquiryHeader);
            _inquiryHeaderRepo.Save();

            foreach (var prod in ProductUserCartVM.ProductList)
            {
                InquiryDetail inquiryDetail = new InquiryDetail()
                {
                    InquiryHeaderId = inquiryHeader.Id,
                    ProductId = prod.Id
                };
                _inquiryDetailRepo.Add(inquiryDetail);
                
            }
            _inquiryDetailRepo.Save();
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

            return RedirectToAction("Index");
        }
    }
}
