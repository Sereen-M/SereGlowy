using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class MakeupProduct
    {
        [Key]
        public int MakeupProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string Brand { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string SuitableSkinType { get; set; }

        public string FinishType { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }
}