using System.ComponentModel.DataAnnotations;

namespace SereGlowy.Models
{
    public class RoutineRecommendation
    {
        [Key]
        public int RoutineRecommendationId { get; set; }

        [Required]
        public string SkinType { get; set; }

        [Required]
        public string SkinGoal { get; set; }

        [Required]
        public string RoutineTime { get; set; }

        public int StepOrder { get; set; }

        [Required]
        public string StepName { get; set; }

        public string Description { get; set; }
    }
}