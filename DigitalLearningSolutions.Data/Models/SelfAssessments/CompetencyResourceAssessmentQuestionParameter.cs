namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class CompetencyResourceAssessmentQuestionParameter
    {
        public int Id { get; set; }

        public int CompetencyLearningResourceId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int MinResultMatch { get; set; }

        public int MaxResultMatch { get; set; }

        public bool Essential { get; set; }

        public int? RelevanceAssessmentQuestionId { get; set; }

        public bool CompareToRoleRequirements { get; set; }
    }
}
