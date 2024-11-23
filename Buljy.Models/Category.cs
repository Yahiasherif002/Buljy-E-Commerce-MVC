using System.ComponentModel.DataAnnotations;

namespace Buljy.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        [MaxLength(20, ErrorMessage = "The Category Name Maximum length is 20")]
        public string Name { get; set; }

        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "The value must be between 1-100")]
        public int DisplayOrder { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
