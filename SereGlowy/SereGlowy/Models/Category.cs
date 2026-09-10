using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<Product>? Products { get; set; }
    }
}