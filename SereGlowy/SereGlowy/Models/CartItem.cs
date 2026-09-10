using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SereGlowy.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}