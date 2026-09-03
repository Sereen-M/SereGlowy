using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SereGlowy.Models
{
    public class MyProduct
    {
        [Key]
        public int MyProductId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}