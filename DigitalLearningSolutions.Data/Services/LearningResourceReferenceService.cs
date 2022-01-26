namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;

    public interface ILearningResourceReferenceService
    {
        string? GetLearningHubResourceLinkByResourceRefId(int learningResourceReferenceId);
    }

    public class LearningResourceReferenceService : ILearningResourceReferenceService
    {
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;

        public LearningResourceReferenceService(
            ILearningResourceReferenceDataService learningResourceReferenceDataService
        )
        {
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
        }

        public string? GetLearningHubResourceLinkByResourceRefId(int learningResourceReferenceId)
        {
            return learningResourceReferenceDataService.GetLearningHubResourceLinkByResourceRefId(
                learningResourceReferenceId
            );
        }
    }
}
