

namespace InstrumentService.Models.ViewModels
{
    public class ProductUserCartVM
    {
        public ProductUserCartVM()
        {
            ProductList = new List<Product>();
        }
        public ApplicationUser ApplicationUser { get; set; }
        public IList<Product> ProductList { get; set; }
    }
}
