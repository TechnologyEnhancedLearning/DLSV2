namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using Microsoft.Extensions.Configuration;

    public interface IActionPlanService
    {
        void AddResourceToActionPlan(int competencyLearningResourceId, int delegateId, int selfAssessmentId);

        Task<IEnumerable<ActionPlanResource>> GetIncompleteActionPlanResources(int delegateId);

        Task<ActionPlanResource?> GetActionPlanResource(int learningLogItemId);

        Task<string?> GetLearningResourceLinkAndUpdateLastAccessedDate(int learningLogItemId, int delegateId);

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate);

        void RemoveActionPlanResource(int learningLogItemId, int delegateId);

        bool? VerifyDelegateCanAccessActionPlanResource(int learningLogItemId, int delegateId);
    }

    public class ActionPlanService : IActionPlanService
    {
        private readonly IClockService clockService;
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        private readonly IConfiguration config;
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly ILearningHubApiService learningHubApiService;
        private readonly ILearningLogItemsDataService learningLogItemsDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public ActionPlanService(
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningLogItemsDataService learningLogItemsDataService,
            IClockService clockService,
            ILearningHubApiService learningHubApiService,
            ILearningHubApiClient learningHubApiClient,
            ISelfAssessmentDataService selfAssessmentDataService,
            IConfiguration config
        )
        {
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningLogItemsDataService = learningLogItemsDataService;
            this.clockService = clockService;
            this.learningHubApiService = learningHubApiService;
            this.learningHubApiClient = learningHubApiClient;
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.config = config;
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
                learningLogItemsDataService.InsertLearningLogItemCompetencies(
                    learningLogItemId,
                    competencyId,
                    addedDate
                );
            }

            transaction.Complete();
        }

        public async Task<IEnumerable<ActionPlanResource>> GetIncompleteActionPlanResources(int delegateId)
        {
            var incompleteLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateId)
                .Where(
                    i => i.CompletedDate == null && i.ArchivedDate == null && i.LearningHubResourceReferenceId != null
                ).ToList();

            if (!incompleteLearningLogItems.Any())
            {
                return new List<ActionPlanResource>();
            }

            var incompleteResourceIds = incompleteLearningLogItems.Select(i => i.LearningHubResourceReferenceId!.Value);
            var bulkResponse = await learningHubApiClient.GetBulkResourcesByReferenceIds(incompleteResourceIds);
            var incompleteActionPlanResources = bulkResponse.ResourceReferences.Select(
                resource => new ActionPlanResource(
                    incompleteLearningLogItems.Single(i => i.LearningHubResourceReferenceId!.Value == resource.RefId),
                    resource
                )
            );
            return incompleteActionPlanResources;
        }

        public async Task<ActionPlanResource?> GetActionPlanResource(int learningLogItemId)
        {
            var learningLogItem = learningLogItemsDataService.GetLearningLogItem(learningLogItemId)!;

            var response =
                await learningHubApiClient.GetResourceByReferenceId(
                    learningLogItem.LearningHubResourceReferenceId!.Value
                );
            return new ActionPlanResource(learningLogItem, response);
        }

        public async Task<string?> GetLearningResourceLinkAndUpdateLastAccessedDate(
            int learningLogItemId,
            int delegateId
        )
        {
            var actionPlanResource = learningLogItemsDataService.GetLearningLogItem(learningLogItemId)!;

            learningLogItemsDataService.UpdateLearningLogItemLastAccessedDate(learningLogItemId, clockService.UtcNow);

            var resource =
                await learningHubApiClient.GetResourceByReferenceId(
                    actionPlanResource.LearningHubResourceReferenceId!.Value
                );

            return resource.Link;
        }

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate)
        {
            learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate);
        }

        public void RemoveActionPlanResource(int learningLogItemId, int delegateId)
        {
            var removalDate = clockService.UtcNow;
            learningLogItemsDataService.RemoveLearningLogItem(learningLogItemId, delegateId, removalDate);
        }

        public bool? VerifyDelegateCanAccessActionPlanResource(int learningLogItemId, int delegateId)
        {
            if (!config.IsSignpostingUsed())
            {
                return null;
            }

            var actionPlanResource = learningLogItemsDataService.GetLearningLogItem(learningLogItemId);

            if (!(actionPlanResource is { ArchivedDate: null }) ||
                actionPlanResource.LearningHubResourceReferenceId == null)
            {
                return null;
            }

            return actionPlanResource.LoggedById == delegateId;
        }
    }
}
