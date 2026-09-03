using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SereGlowy.Models
{
    public class ProductIngredient
    {
        [Key]
        public int ProductIngredientId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        [ForeignKey("Ingredient")]
        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; }
    }
}