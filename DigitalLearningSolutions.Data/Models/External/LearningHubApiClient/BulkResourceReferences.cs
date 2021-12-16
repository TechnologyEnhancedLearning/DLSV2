namespace DigitalLearningSolutions.Data.Models.External.LearningHubApiClient
{
    using System.Collections.Generic;

    public class BulkResourceReferences
    {
        public List<ResourceReferenceWithResourceDetails> ResourceReferences { get; set; }

        public List<int> UnmatchedResourceReferenceIds { get; set; }
    }
}
