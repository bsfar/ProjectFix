using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstrumentService.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Ошибка")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Range (1, int.MaxValue)]
        public double Price { get; set; }
        public string? Image { get; set; }
        public string? ShortDescription { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public int ApplicationTypeId { get; set; }
        [ForeignKey("ApplicationTypeId")]
        public ApplicationType? ApplicationType { get; set; }
    }
}
