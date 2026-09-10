using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class RoutineRecommendation
    {
        [Key]
        public int RoutineRecommendationId { get; set; }

        [Required]
        public string SkinType { get; set; } = string.Empty;

        [Required]
        public string SkinGoal { get; set; } = string.Empty;

        [Required]
        public string RoutineTime { get; set; } = string.Empty;

        public int StepOrder { get; set; }

        [Required]
        public string StepName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}