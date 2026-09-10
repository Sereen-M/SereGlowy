using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class SkinProfile
    {
        [Key]
        public int SkinProfileId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string SkinType { get; set; } = string.Empty;

        public bool? IsSensitive { get; set; }

        [Required]
        public string SkinGoal { get; set; } = string.Empty;

        [Required]
        public string RoutineExperience { get; set; } = string.Empty;

        [Required]
        public string MakeupUsage { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}