namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    using System.Collections.Generic;

    public class ResourceMetadata
    {
        public int ResourceId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<ResourceReference> References { get; set; }

        public string ResourceType { get; set; }

        public decimal Rating { get; set; }
    }
}
