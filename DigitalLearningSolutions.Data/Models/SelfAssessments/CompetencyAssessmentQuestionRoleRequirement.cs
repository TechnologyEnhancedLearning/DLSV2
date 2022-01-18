namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class CompetencyAssessmentQuestionRoleRequirement
    {
        public int Id { get; set; }

        public int SelfAssessmentId { get; set; }

        public int CompetencyId { get; set; }

        public int AssessmentQuestionId { get; set; }

        public int LevelValue { get; set; }

        public int LevelRag { get; set; }
    }
}
