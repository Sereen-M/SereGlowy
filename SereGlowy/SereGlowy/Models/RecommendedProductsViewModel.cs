namespace SereGlowy.Models
{
    public class RecommendedProductsViewModel
    {
        public SkinProfile SkinProfile { get; set; } = new();

        public List<Product> Products { get; set; } = new();
    }
}