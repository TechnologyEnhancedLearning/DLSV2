namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.DataServices;

    public interface ILearningResourceService
    {
        void GetLearningResourcesForDelegateSelfAssessment(int delegateId, int selfAssessmentId);
    }

    public class LearningResourcesService : ILearningResourceService
    {
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public LearningResourcesService(ISelfAssessmentDataService selfAssessmentDataService)
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
        }

        public void GetLearningResourcesForDelegateSelfAssessment(int delegateId, int selfAssessmentId)
        {
            var competencies = selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId);
        }
    }
}
