namespace DigitalLearningSolutions.Data.Models.External.LearningHubApiClient
{
    using System.Collections.Generic;

    public class ResourceMetadata
    {
        public int ResourceId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<ResourceReference> References { get; set; } = new List<ResourceReference>();

        public string ResourceType { get; set; } = string.Empty;

        public decimal Rating { get; set; }
    }
}
