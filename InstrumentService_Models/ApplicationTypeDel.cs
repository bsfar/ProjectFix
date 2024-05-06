using System.ComponentModel.DataAnnotations;

namespace InstrumentService.Models
{
    public class ApplicationTypeDel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Название является обязательным к заполнению")]
        public string Name { get; set; }
    }
}
