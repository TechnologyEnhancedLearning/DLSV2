
using System.ComponentModel.DataAnnotations;

namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class AssessmentQuestionDetail : AssessmentQuestion
    {
        public string? MaxValueDescription { get; set; }
        public string? MinValueDescription { get; set; }
        public bool IncludeComments { get; set; }
        public string? ScoringInstructions { get; set; }
    }
}
