using InstrumentService.Models;
using InstrumentService_DataAccess.Repository.IRepository;
using InstrumentService_Models;
using InstrumentService_Models.ViewModels;
using InstrumentService_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentService.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        private readonly IInquiryDetailRepository _inquiryDetailRepo;
        private readonly IInquiryHeaderRepository _inquiryHeaderRepo;
        [BindProperty]
        public InquiryVM InquiryVM { get; set; }
        public InquiryController(IInquiryDetailRepository inquiryDetailRepo, IInquiryHeaderRepository inquiryHeaderRepo)
        {
            _inquiryDetailRepo = inquiryDetailRepo;
            _inquiryHeaderRepo = inquiryHeaderRepo;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> DetailsAsync(int id)
        {
            InquiryVM = new InquiryVM()
            {
                InquiryHeader = await _inquiryHeaderRepo.FirstOrDefaultAsync(x => x.Id == id),
                InquiryDetail = await _inquiryDetailRepo.GetAllAsync(x => x.InquiryHeaderId == id, includeProperties: "Product")
            };
            return View(InquiryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsAsync()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryVM.InquiryDetail = await _inquiryDetailRepo.GetAllAsync(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            foreach(var detail in InquiryVM.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVM.InquiryHeader.Id);
            return RedirectToAction("Index", "Cart");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync()
        {
            InquiryHeader inquiryHeader = await _inquiryHeaderRepo.FirstOrDefaultAsync(u => u.Id == InquiryVM.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = await _inquiryDetailRepo.GetAllAsync(u => u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);

            _inquiryDetailRepo.RemoveRange(inquiryDetails);
            _inquiryHeaderRepo.Remove(inquiryHeader);
            _inquiryHeaderRepo.Save();

            return RedirectToAction("Index");
        }



        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetInquiryList()
        {
            return Json(new { data = await _inquiryHeaderRepo.GetAllAsync() });
        }
        #endregion
    }
}
