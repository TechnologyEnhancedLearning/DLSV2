namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public interface IRecommendedLearningService
    {
        Task<IEnumerable<RecommendedResource>> GetRecommendedLearningForSelfAssessment(
            int selfAssessmentId,
            int delegateId
        );
    }

    public class RecommendedLearningService : IRecommendedLearningService
    {
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly ILearningLogItemsDataService learningLogItemsDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public RecommendedLearningService(
            ISelfAssessmentDataService selfAssessmentDataService,
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningHubApiClient learningHubApiClient,
            ILearningLogItemsDataService learningLogItemsDataService
        )
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningHubApiClient = learningHubApiClient;
            this.learningLogItemsDataService = learningLogItemsDataService;
        }

        public async Task<IEnumerable<RecommendedResource>> GetRecommendedLearningForSelfAssessment(
            int selfAssessmentId,
            int delegateId
        )
        {
            var competencyIds = selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId);

            var competencyLearningResources = new List<CompetencyLearningResource>();
            foreach (var competencyId in competencyIds)
            {
                var learningHubResourceReferencesForCompetency =
                    competencyLearningResourcesDataService.GetCompetencyLearningResourcesByCompetencyId(
                        competencyId
                    );
                competencyLearningResources.AddRange(learningHubResourceReferencesForCompetency);
            }

            var resourceReferences = competencyLearningResources.Select(
                clr => (clr.LearningHubResourceReferenceId, clr.LearningResourceReferenceId)
            ).Distinct().ToDictionary(x => x.LearningHubResourceReferenceId, x => x.LearningResourceReferenceId);

            var uniqueLearningHubReferenceIds = competencyLearningResources
                .Select(clr => clr.LearningHubResourceReferenceId).Distinct();

            var resources = await learningHubApiClient.GetBulkResourcesByReferenceIds(uniqueLearningHubReferenceIds);

            var delegateLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateId);

            var recommendedResources = resources.ResourceReferences.Select(
                rr =>
                {
                    var learningLogItemsForResource = delegateLearningLogItems.Where(
                        ll => ll.ArchivedDate == null && ll.LearningHubResourceReferenceId == rr.RefId
                    ).ToList();
                    var incompleteLearningLogItem = learningLogItemsForResource.SingleOrDefault(ll => ll.CompletedDate == null);
                    return new RecommendedResource
                    {
                        LearningResourceReferenceId = resourceReferences[rr.RefId],
                        LearningHubReferenceId = rr.RefId,
                        ResourceName = rr.Title,
                        ResourceDescription = rr.Description,
                        ResourceType = rr.ResourceType,
                        CatalogueName = rr.Catalogue.Name,
                        ResourceLink = rr.Link,
                        IsInActionPlan = incompleteLearningLogItem != null,
                        IsCompleted = incompleteLearningLogItem == null && learningLogItemsForResource.Any(ll => ll.CompletedDate != null),
                        LearningLogId = incompleteLearningLogItem?.LearningLogItemId,
                        RecommendationScore = 0, // TODO HEEDLS-705 Calculate this score
                    };
                }
            );

            return recommendedResources;
        }
    }
}
