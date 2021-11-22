namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    public class ResourceMetadata
    {
        public int ResourceId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public ResourceReference[]? References { get; set; }

        public string? ResourceType { get; set; }

        public double Rating { get; set; }
    }
}
