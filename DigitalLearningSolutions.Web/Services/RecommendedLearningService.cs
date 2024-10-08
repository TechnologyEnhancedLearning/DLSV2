﻿namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Constants;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public interface IRecommendedLearningService
    {
        Task<(IEnumerable<RecommendedResource> recommendedResources, bool apiIsAccessible)>
            GetRecommendedLearningForSelfAssessment(
                int selfAssessmentId,
                int delegateUserId
            );
    }

    public class RecommendedLearningService : IRecommendedLearningService
    {
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        private readonly ILearningHubResourceService learningHubResourceService;
        private readonly ILearningLogItemsDataService learningLogItemsDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;
        private readonly IConfigDataService configDataService;

        public RecommendedLearningService(
            ISelfAssessmentDataService selfAssessmentDataService,
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningHubResourceService learningHubResourceService,
            ILearningLogItemsDataService learningLogItemsDataService,
            IConfigDataService configDataService
        )
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningHubResourceService = learningHubResourceService;
            this.learningLogItemsDataService = learningLogItemsDataService;
            this.configDataService = configDataService;
        }

        public async Task<(IEnumerable<RecommendedResource> recommendedResources, bool apiIsAccessible)>
            GetRecommendedLearningForSelfAssessment(
                int selfAssessmentId,
                int delegateUserId
            )
        {
            var hasMaxSignpostedResources = Int32.TryParse(
                configDataService.GetConfigValue(ConfigConstants.MaxSignpostedResources),
                out var maxSignpostedResources
            );

            var competencyIds = selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId);
            var competencyLearningResources = new List<CompetencyLearningResource>();

            foreach (var competencyId in competencyIds)
            {
                var learningHubResourceReferencesForCompetency =
                    competencyLearningResourcesDataService.GetActiveCompetencyLearningResourcesByCompetencyId(
                        competencyId
                    );

                competencyLearningResources.AddRange(learningHubResourceReferencesForCompetency);
            }

            var resourceReferences = competencyLearningResources.Select(
                clr => (clr.LearningHubResourceReferenceId, clr.LearningResourceReferenceId)
            ).Distinct().ToDictionary(
                x => x.LearningHubResourceReferenceId,
                x => x.LearningResourceReferenceId
            );

            var uniqueLearningHubReferenceIds = competencyLearningResources.Select(
                clr => clr.LearningHubResourceReferenceId
            ).Distinct().ToList();

            var resources =
                learningHubResourceService.GetResourceReferenceDetailsByReferenceIds(uniqueLearningHubReferenceIds);

            var delegateLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateUserId);

            var recommendedResources = resources.Select(
                    rr => GetPopulatedRecommendedResource(
                        selfAssessmentId,
                        delegateUserId,
                        resourceReferences[rr.RefId],
                        delegateLearningLogItems,
                        rr,
                        competencyLearningResources
                    )
                )
                .WhereNotNull()
                .OrderByDescending(resource => resource.RecommendationScore);

            var bestRecommendedResources = hasMaxSignpostedResources
                ? recommendedResources.Take(maxSignpostedResources).ToList()
                : recommendedResources.ToList();

            var apiResources =
                await learningHubResourceService.GetBulkResourcesByReferenceIds(
                    bestRecommendedResources.Select(resource => resource.LearningHubReferenceId).ToList()
                );

            var recommendedResourcesPresentInApi = bestRecommendedResources.Where(
                resource => !apiResources.bulkResourceReferences.UnmatchedResourceReferenceIds.Contains(
                    resource.LearningHubReferenceId
                )
            ).ToList();

            return (recommendedResourcesPresentInApi, apiResources.apiIsAccessible);
        }

        private RecommendedResource? GetPopulatedRecommendedResource(
            int selfAssessmentId,
            int delegateUserId,
            int learningHubResourceReferenceId,
            IEnumerable<LearningLogItem> delegateLearningLogItems,
            ResourceReferenceWithResourceDetails rr,
            List<CompetencyLearningResource> competencyLearningResources
        )
        {
            var learningLogItemsForResource = delegateLearningLogItems.Where(
                ll => ll.ArchivedDate == null && ll.LearningHubResourceReferenceId == rr.RefId
            ).ToList();
            var incompleteLearningLogItem =
                learningLogItemsForResource.SingleOrDefault(ll => ll.CompletedDate == null);

            var clrsForResource =
                competencyLearningResources.Where(clr => clr.LearningHubResourceReferenceId == rr.RefId)
                    .ToList();

            var competencyResourceAssessmentQuestionParameters =
                competencyLearningResourcesDataService
                    .GetCompetencyResourceAssessmentQuestionParameters(clrsForResource.Select(clr => clr.Id))
                    .ToList();

            if (!AreDelegateAnswersWithinRangeToDisplayResource(
                clrsForResource,
                competencyResourceAssessmentQuestionParameters,
                selfAssessmentId,
                delegateUserId
            ))
            {
                return null;
            }

            return new RecommendedResource(
                learningHubResourceReferenceId,
                rr,
                incompleteLearningLogItem,
                learningLogItemsForResource.Any(ll => ll.CompletedDate != null),
                CalculateRecommendedLearningScore(
                    rr,
                    clrsForResource,
                    competencyResourceAssessmentQuestionParameters,
                    selfAssessmentId,
                    delegateUserId
                )
            );
        }

        private bool AreDelegateAnswersWithinRangeToDisplayResource(
            List<CompetencyLearningResource> clrsForResource,
            List<CompetencyResourceAssessmentQuestionParameter> competencyResourceAssessmentQuestionParameters,
            int selfAssessmentId,
            int delegateUserId
        )
        {
            foreach (var competencyLearningResource in clrsForResource)
            {
                var delegateResults = selfAssessmentDataService
                    .GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                        delegateUserId,
                        selfAssessmentId,
                        competencyLearningResource.CompetencyId
                    ).ToList();

                var competencyResourceAssessmentQuestionParametersForClr =
                    competencyResourceAssessmentQuestionParameters.SingleOrDefault(
                        qp => qp.CompetencyLearningResourceId == competencyLearningResource.Id
                    );

                if (competencyResourceAssessmentQuestionParametersForClr == null)
                {
                    return true;
                }

                var latestConfidenceResult = delegateResults
                    .Where(
                        dr => dr.AssessmentQuestionId ==
                              competencyResourceAssessmentQuestionParametersForClr.AssessmentQuestionId
                    )
                    .OrderByDescending(dr => dr.DateTime).FirstOrDefault();

                if (competencyResourceAssessmentQuestionParametersForClr.MinResultMatch <=
                    latestConfidenceResult?.Result &&
                    latestConfidenceResult.Result <=
                    competencyResourceAssessmentQuestionParametersForClr.MaxResultMatch)
                {
                    return true;
                }
            }

            return false;
        }

        private decimal CalculateRecommendedLearningScore(
            ResourceReferenceWithResourceDetails resource,
            List<CompetencyLearningResource> clrsForResource,
            List<CompetencyResourceAssessmentQuestionParameter> competencyResourceAssessmentQuestionParameters,
            int selfAssessmentId,
            int delegateUserId
        )
        {
            var essentialnessValue = CalculateEssentialnessValue(competencyResourceAssessmentQuestionParameters);

            var learningHubRating = resource.Rating;

            var requirementAdjuster = CalculateRequirementAdjuster(
                clrsForResource,
                competencyResourceAssessmentQuestionParameters,
                selfAssessmentId,
                delegateUserId
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
            int delegateUserId
        )
        {
            var requirementAdjusters = new List<decimal>();

            foreach (var competencyLearningResource in competencyLearningResources)
            {
                var competencyResourceAssessmentQuestionParametersForClr =
                    competencyResourceAssessmentQuestionParameters.SingleOrDefault(
                        c => c.CompetencyLearningResourceId == competencyLearningResource.Id
                    );

                if (competencyResourceAssessmentQuestionParametersForClr == null)
                {
                    break;
                }

                var delegateResults = selfAssessmentDataService
                    .GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
                        delegateUserId,
                        selfAssessmentId,
                        competencyLearningResource.CompetencyId
                    ).ToList();

                var latestConfidenceResult = delegateResults
                    .Where(
                        dr => dr.AssessmentQuestionId ==
                              competencyResourceAssessmentQuestionParametersForClr.AssessmentQuestionId
                    )
                    .OrderByDescending(dr => dr.DateTime).FirstOrDefault();

                var latestRelevanceResult = delegateResults
                    .Where(
                        dr => dr.AssessmentQuestionId ==
                              competencyResourceAssessmentQuestionParametersForClr.RelevanceAssessmentQuestionId
                    )
                    .OrderByDescending(dr => dr.DateTime).FirstOrDefault();

                if (competencyResourceAssessmentQuestionParametersForClr.CompareToRoleRequirements)
                {
                    requirementAdjusters.Add(
                        CalculateRoleRequirementValue(
                            competencyLearningResource.CompetencyId,
                            selfAssessmentId,
                            latestConfidenceResult
                        )
                    );
                }
                else
                {
                    requirementAdjusters.Add(
                        CalculateRelConValue(
                            latestConfidenceResult,
                            latestRelevanceResult
                        )
                    );
                }
            }

            return requirementAdjusters.Where(ra => ra > 0).Sum();
        }

        private decimal CalculateRoleRequirementValue(
            int competencyId,
            int selfAssessmentId,
            SelfAssessmentResult? latestConfidenceResult
        )
        {
            if (latestConfidenceResult == null)
            {
                return 0;
            }

            var competencyAssessmentQuestionRoleRequirement =
                selfAssessmentDataService.GetCompetencyAssessmentQuestionRoleRequirements(
                    competencyId,
                    selfAssessmentId,
                    latestConfidenceResult.AssessmentQuestionId,
                    latestConfidenceResult.Result
                );

            return (3 - competencyAssessmentQuestionRoleRequirement?.LevelRag) * 25 ?? 0;
        }

        private decimal CalculateRelConValue(
            SelfAssessmentResult? latestConfidenceResult,
            SelfAssessmentResult? latestRelevanceResult
        )
        {
            if (latestConfidenceResult != null && latestRelevanceResult != null)
            {
                return (latestRelevanceResult.Result - latestConfidenceResult.Result) * 10;
            }

            return 0;
        }
    }
}
