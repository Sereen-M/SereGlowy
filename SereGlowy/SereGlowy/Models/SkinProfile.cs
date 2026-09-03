using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class SkinProfile
    {
        [Key]
        public int SkinProfileId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string SkinType { get; set; }

        public bool? IsSensitive { get; set; }

        [Required]
        public string SkinGoal { get; set; }

        [Required]
        public string RoutineExperience { get; set; }

        [Required]
        public string MakeupUsage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}