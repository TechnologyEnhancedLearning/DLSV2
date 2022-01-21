namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public interface ILearningHubResourceService
    {
        Task<(ResourceReferenceWithResourceDetails? resource, bool sourcedFromFallbackData)> GetResourceByReferenceId(
            int resourceReferenceId
        );

        Task<(BulkResourceReferences bulkResourceReferences, bool sourcedFromFallbackData)>
            GetBulkResourcesByReferenceIds(
                IList<int> resourceReferenceIds
            );
    }

    public class LearningHubResourceService : ILearningHubResourceService
    {
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;

        public LearningHubResourceService(
            ILearningHubApiClient learningHubApiClient,
            ILearningResourceReferenceDataService learningResourceReferenceDataService
        )
        {
            this.learningHubApiClient = learningHubApiClient;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
        }

        public async Task<(ResourceReferenceWithResourceDetails? resource, bool sourcedFromFallbackData)>
            GetResourceByReferenceId(
                int resourceReferenceId
            )
        {
            try
            {
                var upToDateResourceDetails = await learningHubApiClient.GetResourceByReferenceId(resourceReferenceId);
                return (upToDateResourceDetails, false);
            }
            catch (LearningHubResponseException e)
            {
                if (e.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    return (null, false);
                }

                var fallBackResourceDetails =
                    learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(
                        new[] { resourceReferenceId }
                    ).SingleOrDefault();

                return (fallBackResourceDetails, true);
            }
        }

        public async Task<(BulkResourceReferences bulkResourceReferences, bool sourcedFromFallbackData)>
            GetBulkResourcesByReferenceIds(
                IList<int> resourceReferenceIds
            )
        {
            try
            {
                var upToDateResourceDetails =
                    await learningHubApiClient.GetBulkResourcesByReferenceIds(resourceReferenceIds);
                return (upToDateResourceDetails, false);
            }
            catch (LearningHubResponseException)
            {
                var fallBackResources =
                    learningResourceReferenceDataService.GetResourceReferenceDetailsByReferenceIds(resourceReferenceIds)
                        .ToList();

                var bulkResourceReferences = new BulkResourceReferences
                {
                    ResourceReferences = fallBackResources,
                    UnmatchedResourceReferenceIds = resourceReferenceIds.Except(
                        fallBackResources.Select(r => r.RefId)
                    ).ToList(),
                };
                return (bulkResourceReferences, true);
            }
        }
    }
}
