namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    using System.ComponentModel.DataAnnotations;
    public class FrameworkCompetency
    {
        public int Id { get; set; }
        public int CompetencyID { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Ordering { get; set; }
        public int AssessmentQuestions { get; set; }
        public int CompetencyLearningResourcesCount { get; set; }
    }
}
