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

                logger.LogWarning(
                    $"Attempting to use fallback data for single resource reference ID {resourceReferenceId}"
                );
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
                logger.LogWarning(
                    $"Attempting to use fallback data for resource references Ids: {DisplayListOfResourceReferenceIds(resourceReferenceIds)}"
                );

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

        private static string DisplayListOfResourceReferenceIds(IEnumerable<int> resourceReferenceIds)
        {
            return new StringBuilder().AppendJoin(", ", resourceReferenceIds.OrderBy(i => i)).ToString();
        }
    }
}
