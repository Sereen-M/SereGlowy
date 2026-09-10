using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class MakeupProduct
    {
        [Key]
        public int MakeupProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string Brand { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string SuitableSkinType { get; set; } = string.Empty;

        public string FinishType { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}