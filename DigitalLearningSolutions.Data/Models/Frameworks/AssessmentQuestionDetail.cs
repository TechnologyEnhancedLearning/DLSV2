
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class AssessmentQuestionDetail : AssessmentQuestion
    {
        [Required]
        public string MaxValueDescription { get; set; }
        [Required]
        public string MinValueDescription { get; set; }
        public int AssessmentQuestionInputTypeID { get; set; }
        public bool IncludeComments { get; set; }
        public string ScoringInstructions { get; set; }
    }
}
