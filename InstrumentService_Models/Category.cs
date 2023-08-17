using System.ComponentModel.DataAnnotations;

namespace InstrumentService.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Название является обязательным для заполнения")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Порядок отображения является обязательным для заполнения")]
        [Range(1, int.MaxValue, ErrorMessage ="Порядок отображения должен быть больше 0")]
        public int? DisplayOrder { get; set; }
       
    }
}
