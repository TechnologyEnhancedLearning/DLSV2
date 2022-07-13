namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface IActionPlanService
    {
        Task AddResourceToActionPlan(int learningResourceReferenceId, int delegateId, int selfAssessmentId);

        Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)> GetIncompleteActionPlanResources(
            int delegateId
        );

        Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)> GetCompletedActionPlanResources(
            int delegateId
        );

        Task<(ActionPlanResource? actionPlanResource, bool apiIsAccessible)> GetActionPlanResource(
            int learningLogItemId
        );

        void UpdateActionPlanResourcesLastAccessedDateIfPresent(int resourceReferenceId, int delegateId);

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate);

        public void SetCompleteByDate(int learningLogItemId, DateTime? completeByDate);

        void RemoveActionPlanResource(int learningLogItemId, int delegateId);

        bool? VerifyDelegateCanAccessActionPlanResource(int learningLogItemId, int delegateId);

        bool ResourceCanBeAddedToActionPlan(int resourceReferenceId, int delegateId);
    }

    public class ActionPlanService : IActionPlanService
    {
        private readonly IClockUtility clockUtility;
        private readonly ICompetencyLearningResourcesDataService competencyLearningResourcesDataService;
        private readonly IConfiguration config;
        private readonly ILearningHubResourceService learningHubResourceService;
        private readonly ILearningLogItemsDataService learningLogItemsDataService;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public ActionPlanService(
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningLogItemsDataService learningLogItemsDataService,
            IClockUtility clockUtility,
            ILearningHubResourceService learningHubResourceService,
            ISelfAssessmentDataService selfAssessmentDataService,
            IConfiguration config,
            ILearningResourceReferenceDataService learningResourceReferenceDataService
        )
        {
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningLogItemsDataService = learningLogItemsDataService;
            this.clockUtility = clockUtility;
            this.learningHubResourceService = learningHubResourceService;
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.config = config;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
        }

        public async Task AddResourceToActionPlan(int learningResourceReferenceId, int delegateId, int selfAssessmentId)
        {
            var learningHubResourceReferenceId =
                learningResourceReferenceDataService.GetLearningHubResourceReferenceById(learningResourceReferenceId);

            var competenciesForResource =
                competencyLearningResourcesDataService.GetCompetencyIdsLinkedToResource(
                    learningResourceReferenceId
                );

            var selfAssessmentCompetencies =
                selfAssessmentDataService.GetCompetencyIdsForSelfAssessment(selfAssessmentId);

            var learningLogCompetenciesToAdd =
                competenciesForResource.Where(id => selfAssessmentCompetencies.Contains(id));

            var addedDate = clockUtility.UtcNow;

            var (resource, apiIsAccessible) =
                await learningHubResourceService.GetResourceByReferenceId(
                    learningHubResourceReferenceId
                );

            if (resource == null)
            {
                throw new ResourceNotFoundException(
                    "Item cannot be added to action plan.",
                    apiIsAccessible
                );
            }

            using var transaction = new TransactionScope();

            var learningLogItemId = learningLogItemsDataService.InsertLearningLogItem(
                delegateId,
                addedDate,
                resource.Title,
                resource.Link,
                learningResourceReferenceId
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

        public async Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)>
            GetIncompleteActionPlanResources(int delegateId)
        {
            var incompleteLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateId)
                .Where(
                    i => i.CompletedDate == null && i.ArchivedDate == null
                ).ToList();

            return await MapLearningLogItemsToActionPlanResources(incompleteLearningLogItems);
        }

        public async Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)>
            GetCompletedActionPlanResources(int delegateId)
        {
            var completedLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateId)
                .Where(
                    i => i.CompletedDate != null && i.ArchivedDate == null
                ).ToList();

            return await MapLearningLogItemsToActionPlanResources(completedLearningLogItems);
        }

        public async Task<(ActionPlanResource? actionPlanResource, bool apiIsAccessible)> GetActionPlanResource(
            int learningLogItemId
        )
        {
            var learningLogItem = learningLogItemsDataService.GetLearningLogItem(learningLogItemId)!;

            var (resource, apiIsAccessible) = await learningHubResourceService
                .GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(
                    learningLogItem.LearningHubResourceReferenceId!.Value
                );

            return resource != null
                ? (new ActionPlanResource(learningLogItem, resource), apiIsAccessible)
                : (null, apiIsAccessible);
        }

        public void UpdateActionPlanResourcesLastAccessedDateIfPresent(
            int resourceReferenceId,
            int delegateId
        )
        {
            var actionPlanResourcesToUpdate = learningLogItemsDataService.GetLearningLogItems(delegateId).Where(
                r =>
                    r.ArchivedDate == null &&
                    r.LearningHubResourceReferenceId == resourceReferenceId
            );

            foreach (var actionPlanResourceToUpdate in actionPlanResourcesToUpdate)
            {
                learningLogItemsDataService.UpdateLearningLogItemLastAccessedDate(
                    actionPlanResourceToUpdate.LearningLogItemId,
                    clockUtility.UtcNow
                );
            }
        }

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate)
        {
            learningLogItemsDataService.SetCompletionDate(learningLogItemId, completedDate);
        }

        public void SetCompleteByDate(int learningLogItemId, DateTime? completeByDate)
        {
            learningLogItemsDataService.SetCompleteByDate(learningLogItemId, completeByDate);
        }

        public void RemoveActionPlanResource(int learningLogItemId, int delegateId)
        {
            var removalDate = clockUtility.UtcNow;
            learningLogItemsDataService.RemoveLearningLogItem(learningLogItemId, delegateId, removalDate);
        }

        public bool? VerifyDelegateCanAccessActionPlanResource(int learningLogItemId, int delegateId)
        {
            if (!ConfigurationExtensions.IsSignpostingUsed(config))
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

        public bool ResourceCanBeAddedToActionPlan(int resourceReferenceId, int delegateId)
        {
            var incompleteLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateId)
                .Where(
                    i => i.CompletedDate == null && i.ArchivedDate == null && i.LearningHubResourceReferenceId != null
                ).ToList();

            return incompleteLearningLogItems.All(i => i.LearningResourceReferenceId != resourceReferenceId);
        }

        private async Task<(IEnumerable<ActionPlanResource>, bool apiIsAccessible)>
            MapLearningLogItemsToActionPlanResources(
                IEnumerable<LearningLogItem> learningLogItems
            )
        {
            var learningLogItemsWithResourceReferencesIds =
                learningLogItems.Where(i => i.LearningHubResourceReferenceId != null).ToList();

            if (!learningLogItemsWithResourceReferencesIds.Any())
            {
                return (new List<ActionPlanResource>(), true);
            }

            var resourceIds = learningLogItemsWithResourceReferencesIds
                .Select(i => i.LearningHubResourceReferenceId!.Value).Distinct().ToList();
            var (bulkResourceReferences, apiIsAccessible) = await learningHubResourceService
                .GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(resourceIds);
            var matchingLearningLogItems = learningLogItemsWithResourceReferencesIds.Where(
                i => !bulkResourceReferences.UnmatchedResourceReferenceIds.Contains(
                    i.LearningHubResourceReferenceId!.Value
                )
            );

            var actionPlanResources = matchingLearningLogItems.Select(
                learningLogItem =>
                {
                    var matchingResource = bulkResourceReferences.ResourceReferences.Single(
                        resource => resource.RefId == learningLogItem.LearningHubResourceReferenceId
                    );
                    return new ActionPlanResource(learningLogItem, matchingResource);
                }
            ).Where(r => r != null);
            return (actionPlanResources, apiIsAccessible);
        }
    }
}
