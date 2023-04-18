namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface IActionPlanService
    {
        Task AddResourceToActionPlan(int learningResourceReferenceId, int delegateId, int selfAssessmentId);

        Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)> GetIncompleteActionPlanResources(
            int delegateUserId
        );

        Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)> GetCompletedActionPlanResources(
            int delegateUserId
        );

        Task<(ActionPlanResource? actionPlanResource, bool apiIsAccessible)> GetActionPlanResource(
            int learningLogItemId
        );

        void UpdateActionPlanResourcesLastAccessedDateIfPresent(int resourceReferenceId, int delegateUserId);

        public void SetCompletionDate(int learningLogItemId, DateTime completedDate);

        public void SetCompleteByDate(int learningLogItemId, DateTime? completeByDate);

        void RemoveActionPlanResource(int learningLogItemId, int delegateId);

        bool? VerifyDelegateCanAccessActionPlanResource(int learningLogItemId, int delegateId);

        bool ResourceCanBeAddedToActionPlan(int resourceReferenceId, int delegateUserId);
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
        private readonly IUserDataService userDataService;

        public ActionPlanService(
            ICompetencyLearningResourcesDataService competencyLearningResourcesDataService,
            ILearningLogItemsDataService learningLogItemsDataService,
            IClockUtility clockUtility,
            ILearningHubResourceService learningHubResourceService,
            ISelfAssessmentDataService selfAssessmentDataService,
            IConfiguration config,
            ILearningResourceReferenceDataService learningResourceReferenceDataService,
            IUserDataService userDataService
        )
        {
            this.competencyLearningResourcesDataService = competencyLearningResourcesDataService;
            this.learningLogItemsDataService = learningLogItemsDataService;
            this.clockUtility = clockUtility;
            this.learningHubResourceService = learningHubResourceService;
            this.selfAssessmentDataService = selfAssessmentDataService;
            this.config = config;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
            this.userDataService = userDataService;
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

            // We can assume a single Candidate Assessment because we'll be adding a uniqueness constraint
            // on CandidateAssessments (candidateId, selfAssessmentId) before releasing (see HEEDLS-932)
            var delegateUserId = userDataService.GetUserIdFromDelegateId(delegateId);
            var candidateAssessmentIdIfAny = selfAssessmentDataService
                .GetCandidateAssessments(delegateUserId, selfAssessmentId)
                .SingleOrDefault()?.Id;

            if (candidateAssessmentIdIfAny == null)
            {
                throw new InvalidOperationException(
                    $"Cannot add resource to action plan as user {delegateUserId} is not enrolled on self assessment {selfAssessmentId}"
                );
            }

            learningLogItemsDataService.InsertCandidateAssessmentLearningLogItem(
                candidateAssessmentIdIfAny!.Value,
                learningLogItemId
            );

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
            GetIncompleteActionPlanResources(int delegateUserId)
        {
            var incompleteLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateUserId)
                .Where(
                    i => i.CompletedDate == null && i.ArchivedDate == null
                ).ToList();

            return await MapLearningLogItemsToActionPlanResources(incompleteLearningLogItems);
        }

        public async Task<(IEnumerable<ActionPlanResource> resources, bool apiIsAccessible)>
            GetCompletedActionPlanResources(int delegateUserId)
        {
            var completedLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateUserId)
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
            int delegateUserId
        )
        {
            var actionPlanResourcesToUpdate = learningLogItemsDataService.GetLearningLogItems(delegateUserId).Where(
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

        public bool ResourceCanBeAddedToActionPlan(int resourceReferenceId, int delegateUserId)
        {
            var incompleteLearningLogItems = learningLogItemsDataService.GetLearningLogItems(delegateUserId)
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
