using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required]
        public string IngredientName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Purpose { get; set; } = string.Empty;

        public string SuitableSkinType { get; set; } = string.Empty;

        public List<ProductIngredient>? ProductIngredients { get; set; }
    }
}