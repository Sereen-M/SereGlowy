using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public List<Product> Products { get; set; }
    }
}