using System.ComponentModel.DataAnnotations;

namespace InstrumentService.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Название является обязательным к заполнению")]
        public string Name { get; set; }
    }
}
