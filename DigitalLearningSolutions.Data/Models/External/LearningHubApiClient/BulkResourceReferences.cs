namespace DigitalLearningSolutions.Data.Models.External.LearningHubApiClient
{
    using System.Collections.Generic;

    public class BulkResourceReferences
    {
        public List<ResourceReferenceWithResourceDetails> ResourceReferences { get; set; } = new List<ResourceReferenceWithResourceDetails>(); 

        public List<int> UnmatchedResourceReferenceIds { get; set; } = new List<int>();
    }
}
