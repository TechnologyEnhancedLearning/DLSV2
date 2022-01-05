namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class CompetencyAssessmentQuestion
    {
        public int CompetencyId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int Ordering { get; set; }

        public bool Required { get; set; }
    }
}
