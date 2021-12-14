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
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public RecommendedLearningService(
            ISelfAssessmentDataService selfAssessmentDataService,
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningHubApiClient learningHubApiClient
        )
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningHubApiClient = learningHubApiClient;
        }

        public async Task<IEnumerable<RecommendedResource>> GetRecommendedLearningForSelfAssessment(
            int selfAssessmentId,
            int delegateId
        )
        {
            var competencyIds = selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId);

            var allLearningHubReferenceIds = new List<int>();
            foreach (var competencyId in competencyIds)
            {
                var learningHubResourceReferencesForCompetency =
                    competencyLearningResourcesDataService.GetLearningHubResourceReferenceIdsByCompetencyId(
                        competencyId
                    );
                allLearningHubReferenceIds.AddRange(learningHubResourceReferencesForCompetency);
            }

            var uniqueLearningHubReferenceIds = allLearningHubReferenceIds.Distinct();

            var resources = await learningHubApiClient.GetBulkResourcesByReferenceIds(uniqueLearningHubReferenceIds);

            var recommendedResources = resources.ResourceReferences.Select(
                rr =>
                {
                    var otherCompetenciesForResource =
                        competencyLearningResourcesDataService.GetCompetencyLearningResourceIdsByLearningHubResourceReference(rr.RefId);
                    return new RecommendedResource
                    {
                        LearningHubReferenceId = rr.RefId,
                        ResourceName = rr.Title,
                        ResourceDescription = rr.Description,
                        ResourceType = rr.ResourceType,
                        CatalogueName = rr.Catalogue.Name,
                        ResourceLink = rr.Link,
                        IsInActionPlan = false,
                        IsCompleted = false,
                        RecommendationScore = 0,
                    };
                }
            );

            return recommendedResources;
        }
    }
}
