namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

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
                    var incompleteLearningLogItem =
                        learningLogItemsForResource.SingleOrDefault(ll => ll.CompletedDate == null);
                    return new RecommendedResource(
                        resourceReferences[rr.RefId],
                        rr,
                        incompleteLearningLogItem,
                        learningLogItemsForResource.Any(ll => ll.CompletedDate != null),
                        CalculateRecommendedLearningScore(rr, competencyLearningResources, selfAssessmentId, delegateId)
                    );
                }
            );

            return recommendedResources;
        }

        private decimal CalculateRecommendedLearningScore(
            ResourceReferenceWithResourceDetails resource,
            List<CompetencyLearningResource> competencyLearningResources,
            int selfAssessmentId,
            int delegateId
        )
        {
            var clrsForResource =
                competencyLearningResources.Where(clr => clr.LearningHubResourceReferenceId == resource.RefId).ToList();

            var competencyResourceAssessmentQuestionParameters =
                competencyLearningResourcesDataService
                    .GetCompetencyResourceAssessmentQuestionParameters(clrsForResource.Select(clr => clr.Id)).ToList();

            var essentialnessValue = CalculateEssentialnessValue(competencyResourceAssessmentQuestionParameters);

            var learningHubRating = resource.Rating;

            var requirementAdjuster = CalculateRequirementAdjuster(
                clrsForResource,
                competencyResourceAssessmentQuestionParameters,
                selfAssessmentId,
                delegateId
            );

            return essentialnessValue + learningHubRating * 4 + requirementAdjuster;
        }

        private static int CalculateEssentialnessValue(
            List<CompetencyResourceAssessmentQuestionParameter> competencyResourceAssessmentQuestionParameters
        )
        {
            return !competencyResourceAssessmentQuestionParameters.Any() ? 0 :
                competencyResourceAssessmentQuestionParameters.Any(aqp => aqp.Essential) ? 100 : 30;
        }

        private decimal CalculateRequirementAdjuster(
            List<CompetencyLearningResource> competencyLearningResources,
            List<CompetencyResourceAssessmentQuestionParameter> competencyResourceAssessmentQuestionParameters,
            int selfAssessmentId,
            int delegateId
        )
        {
            var requirementAdjusters = new List<decimal>();

            foreach (var competencyLearningResource in competencyLearningResources)
            {
                var competencyResourceAssessmentQuestionParameterForClr =
                    competencyResourceAssessmentQuestionParameters.SingleOrDefault(
                        c => c.CompetencyLearningResourceId == competencyLearningResource.Id
                    );

                if (competencyResourceAssessmentQuestionParameterForClr == null)
                {
                    break;
                }

                if (competencyResourceAssessmentQuestionParameterForClr.CompareToRoleRequirements)
                {
                    requirementAdjusters.Add(
                        CalculateRoleRequirementValue(competencyLearningResource.CompetencyId, selfAssessmentId)
                    );
                }
                else
                {
                    requirementAdjusters.Add(
                        CalculateRelConValue(
                            competencyLearningResource
                                .CompetencyId,
                            selfAssessmentId,
                            delegateId,
                            competencyResourceAssessmentQuestionParameterForClr
                        )
                    );
                }
            }

            return requirementAdjusters.Where(ra => ra > 0).Sum();
        }

        private decimal CalculateRoleRequirementValue(int competencyId, int selfAssessmentId)
        {
            var competencyAssessmentQuestionRoleRequirement =
                selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    competencyId,
                    selfAssessmentId
                );

            return (3 - competencyAssessmentQuestionRoleRequirement?.LevelRag) * 25 ?? 0;
        }

        private decimal CalculateRelConValue(
            int competencyId,
            int selfAssessmentId,
            int delegateId,
            CompetencyResourceAssessmentQuestionParameter competencyResourceAssessmentQuestionParameter
        )
        {
            var delegateResults = selfAssessmentDataService
                .GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                    delegateId,
                    selfAssessmentId,
                    competencyId
                ).ToList();

            var latestConfidenceResult = delegateResults
                .Where(
                    dr => dr.AssessmentQuestionId == competencyResourceAssessmentQuestionParameter.AssessmentQuestionId
                )
                .OrderByDescending(dr => dr.DateTime).FirstOrDefault();

            var latestRelevanceResult = delegateResults
                .Where(
                    dr => dr.AssessmentQuestionId ==
                          competencyResourceAssessmentQuestionParameter.RelevanceAssessmentQuestionId
                )
                .OrderByDescending(dr => dr.DateTime).FirstOrDefault();

            if (latestConfidenceResult != null && latestRelevanceResult != null)
            {
                return (latestRelevanceResult.Result - latestConfidenceResult.Result) * 10;
            }

            return 0;
        }
    }
}
