namespace InstrumentService.Models.ViewModels
{
    public class DetailsVM
    {
        //добавлем конструктор по умолчанию, где инициализируем сразу же Product
        //это нужно для того, что бы в контроллере не пришлось это делать
        public DetailsVM()
        {
            Product = new Product();
        }
        public Product Product { get; set; }
        public bool ExistsInCart { get; set; }
    }
}
