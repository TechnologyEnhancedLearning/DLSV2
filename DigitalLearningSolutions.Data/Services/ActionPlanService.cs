namespace DigitalLearningSolutions.Data.Services
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;

    public interface IActionPlanService
    {
        void AddResourceToActionPlan(int competencyLearningResourceId, int delegateId, int selfAssessmentId);
    }

    public class ActionPlanService : IActionPlanService
    {
        private readonly IClockService clockService;
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        private readonly ILearningHubApiService learningHubApiService;
        private readonly ILearningLogItemsDataService learningLogItemsDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public ActionPlanService(
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningLogItemsDataService learningLogItemsDataService,
            IClockService clockService,
            ILearningHubApiService learningHubApiService,
            ISelfAssessmentDataService selfAssessmentDataService
        )
        {
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningLogItemsDataService = learningLogItemsDataService;
            this.clockService = clockService;
            this.learningHubApiService = learningHubApiService;
            this.selfAssessmentDataService = selfAssessmentDataService;
        }

        public void AddResourceToActionPlan(int competencyLearningResourceId, int delegateId, int selfAssessmentId)
        {
            var competencyLearningResource =
                competencyLearningResourcesDataService.GetCompetencyLearningResourceById(competencyLearningResourceId);

            var (resourceName, resourceLink) =
                learningHubApiService.GetResourceNameAndLink(competencyLearningResource.LearningHubResourceReferenceId);

            var otherCompetenciesForResource =
                competencyLearningResourcesDataService.GetCompetencyIdsByLearningHubResourceReference(
                    competencyLearningResource.LearningHubResourceReferenceId
                );

            var selfAssessmentCompetencies =
                selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId);

            var learningLogCompetenciesToAdd =
                otherCompetenciesForResource.Where(id => selfAssessmentCompetencies.Contains(id));

            var addedDate = clockService.UtcNow;

            using var transaction = new TransactionScope();

            var learningLogItemId = learningLogItemsDataService.InsertLearningLogItem(
                delegateId,
                addedDate,
                resourceName,
                resourceLink,
                competencyLearningResourceId
            );

            learningLogItemsDataService.InsertCandidateAssessmentLearningLogItem(selfAssessmentId, learningLogItemId);

            foreach (var competencyId in learningLogCompetenciesToAdd)
            {
                learningLogItemsDataService.InsertLearningLogItemCompetencies(learningLogItemId, competencyId, addedDate);
            }

            transaction.Complete();
        }
    }
}
