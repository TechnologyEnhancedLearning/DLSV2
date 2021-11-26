namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    using System.Collections.Generic;

    public class BulkResourceReference
    {
        public List<ResourceReferenceWithResourceDetails> ResourceReferences { get; set; }

        public List<int> UnmatchedResourceReferenceIds { get; set; }
    }
}
