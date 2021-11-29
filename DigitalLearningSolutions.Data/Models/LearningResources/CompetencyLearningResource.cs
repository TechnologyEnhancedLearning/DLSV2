namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    public class CompetencyLearningResource
    {
        public int Id { get; set; }

        public int CompetencyId { get; set; }

        public int LearningHubResourceReferenceId { get; set; }

        public int AdminId { get; set; }
    }
}
