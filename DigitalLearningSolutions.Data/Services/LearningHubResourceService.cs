namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using Microsoft.Extensions.Logging;

    public interface ILearningHubResourceService
    {
        Task<LearningResourceReferenceWithResourceDetails?> GetResourceByReferenceId(int resourceReferenceId);

        Task<BulkLearningResourceReferences> GetBulkResourcesByReferenceIds(
            IList<int> resourceReferenceIds
        );
    }

    public class LearningHubResourceService : ILearningHubResourceService
    {
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;
        private readonly ILogger<LearningHubResourceService> logger;

        public LearningHubResourceService(ILearningHubApiClient learningHubApiClient, ILearningResourceReferenceDataService learningResourceReferenceDataService, ILogger<LearningHubResourceService> logger)
        {
            this.learningHubApiClient = learningHubApiClient;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
            this.logger = logger;
        }

        public async Task<LearningResourceReferenceWithResourceDetails?> GetResourceByReferenceId(int resourceReferenceId)
        {
            try
            {
                // TODO HEEDLS-747: tackle different response codes rather than catching all exceptions
                var upToDateResourceDetails = await learningHubApiClient.GetResourceByReferenceId(resourceReferenceId);
                return new LearningResourceReferenceWithResourceDetails(upToDateResourceDetails, false);
            }
            catch (Exception e)
            {
                logger.LogWarning($"Call to Learning Hub Open API failed when trying to call /Resource/{resourceReferenceId} endpoint. " +
                                  $"Using fall-back data instead. Exception: {e.Message}");

                var fallBackResourceDetails =
                    learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                        new[] { resourceReferenceId }
                    ).SingleOrDefault();

                return fallBackResourceDetails == null
                    ? null
                    : new LearningResourceReferenceWithResourceDetails(
                        fallBackResourceDetails,
                        true
                    );
            }
        }

        public async Task<BulkLearningResourceReferences> GetBulkResourcesByReferenceIds(IList<int> resourceReferenceIds)
        {
            try
            {
                // TODO HEEDLS-747: tackle different response codes rather than catching all exceptions
                var upToDateResourceDetails = await learningHubApiClient.GetBulkResourcesByReferenceIds(resourceReferenceIds);
                return new BulkLearningResourceReferences(upToDateResourceDetails, false);
            }
            catch (Exception e)
            {
                logger.LogWarning("Call to Learning Hub Open API failed when trying to call /Resource/Bulk endpoint. " +
                                  $"Using fall-back data instead. Exception: {e.Message}");

                var fallBackResources =
                    learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(resourceReferenceIds).ToList();

                var bulkResourceReferences = new BulkResourceReferences
                {
                    ResourceReferences = fallBackResources,
                    UnmatchedResourceReferenceIds = resourceReferenceIds.Where(
                        refId => fallBackResources.All(resource => resource.RefId != refId)
                    ).ToList()
                };
                return new BulkLearningResourceReferences(bulkResourceReferences, true);
            }
        }
    }
}
