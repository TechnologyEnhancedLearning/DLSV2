namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using Microsoft.Extensions.Logging;

    public interface ILearningHubResourceService
    {
        Task<(ResourceReferenceWithResourceDetails? resource, bool apiIsAccessible)> GetResourceByReferenceId(
            int resourceReferenceId
        );

        Task<(ResourceReferenceWithResourceDetails? resource, bool apiIsAccessible)>
            GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(
                int resourceReferenceId
            );

        Task<(BulkResourceReferences bulkResourceReferences, bool apiIsAccessible)> GetBulkResourcesByReferenceIds(
            IList<int> resourceReferenceIds
        );

        Task<(BulkResourceReferences bulkResourceReferences, bool apiIsAccessible)>
            GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(IList<int> resourceReferenceIds);
    }

    public class LearningHubResourceService : ILearningHubResourceService
    {
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;
        private readonly ILogger<ILearningHubResourceService> logger;

        public LearningHubResourceService(
            ILearningHubApiClient learningHubApiClient,
            ILearningResourceReferenceDataService learningResourceReferenceDataService,
            ILogger<ILearningHubResourceService> logger
        )
        {
            this.learningHubApiClient = learningHubApiClient;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
            this.logger = logger;
        }

        public async Task<(ResourceReferenceWithResourceDetails? resource, bool apiIsAccessible)>
            GetResourceByReferenceId(int resourceReferenceId)
        {
            try
            {
                var upToDateResourceDetails = await learningHubApiClient.GetResourceByReferenceId(resourceReferenceId);
                return (upToDateResourceDetails, true);
            }
            catch (LearningHubResponseException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    return (null, true);
                }

                var fallBackResourceDetails =
                    GetFallbackDataForResourceReferenceIds(new List<int> { resourceReferenceId }).SingleOrDefault();

                return (fallBackResourceDetails, false);
            }
        }

        public async Task<(ResourceReferenceWithResourceDetails? resource, bool apiIsAccessible)>
            GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(int resourceReferenceId)
        {
            var (resource, apiIsAccessible) = await GetResourceByReferenceId(resourceReferenceId);

            if (resource != null || !apiIsAccessible)
            {
                return (resource, apiIsAccessible);
            }

            var fallBackResourceDetails =
                GetFallbackDataForResourceReferenceIds(new List<int> { resourceReferenceId }).SingleOrDefault();

            if (fallBackResourceDetails != null)
            {
                fallBackResourceDetails.AbsentInLearningHub = true;
            }

            return (fallBackResourceDetails, true);
        }

        public async Task<(BulkResourceReferences bulkResourceReferences, bool apiIsAccessible)>
            GetBulkResourcesByReferenceIds(
                IList<int> resourceReferenceIds
            )
        {
            try
            {
                var bulkApiResponse =
                    await learningHubApiClient.GetBulkResourcesByReferenceIds(resourceReferenceIds);
                return (bulkApiResponse, true);
            }
            catch (LearningHubResponseException)
            {
                var fallbackResources = GetFallbackDataForResourceReferenceIds(resourceReferenceIds).ToList();

                var bulkResourceReferences = new BulkResourceReferences
                {
                    ResourceReferences = fallbackResources,
                    UnmatchedResourceReferenceIds = resourceReferenceIds.Except(
                        fallbackResources.Select(r => r.RefId)
                    ).ToList(),
                };
                return (bulkResourceReferences, false);
            }
        }

        public async Task<(BulkResourceReferences bulkResourceReferences, bool apiIsAccessible)>
            GetBulkResourcesByReferenceIdsAndPopulateDeletedDetailsFromDatabase(IList<int> resourceReferenceIds)
        {
            var (bulkResourceResponse, apiIsAccessible) =
                await GetBulkResourcesByReferenceIds(resourceReferenceIds);

            if (!bulkResourceResponse.UnmatchedResourceReferenceIds.Any() || !apiIsAccessible)
            {
                return (bulkResourceResponse, apiIsAccessible);
            }

            var deletedFallbackResources =
                GetFallbackDataForResourceReferenceIds(bulkResourceResponse.UnmatchedResourceReferenceIds).ToList();

            foreach (var resource in deletedFallbackResources)
            {
                resource.AbsentInLearningHub = true;
            }

            bulkResourceResponse.ResourceReferences.AddRange(deletedFallbackResources);
            bulkResourceResponse.UnmatchedResourceReferenceIds = resourceReferenceIds.Except(
                bulkResourceResponse.ResourceReferences.Select(r => r.RefId)
            ).ToList();

            return (bulkResourceResponse, true);
        }

        private IEnumerable<ResourceReferenceWithResourceDetails> GetFallbackDataForResourceReferenceIds(
            IList<int> resourceReferenceIds
        )
        {
            var commaSeparatedListOfIds =
                new StringBuilder().AppendJoin(", ", resourceReferenceIds.OrderBy(i => i)).ToString();
            logger.LogWarning(
                $"Attempting to use fallback data for resource references Ids: {commaSeparatedListOfIds}"
            );

            return learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(resourceReferenceIds);
        }
    }
}
