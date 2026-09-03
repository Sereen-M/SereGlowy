using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        public string IngredientName { get; set; }

        public string Description { get; set; }

        public string Purpose { get; set; }

        public string SuitableSkinType { get; set; }

        public List<ProductIngredient> ProductIngredients { get; set; }
    }
}