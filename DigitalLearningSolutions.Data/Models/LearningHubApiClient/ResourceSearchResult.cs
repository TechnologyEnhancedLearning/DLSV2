namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    public class ResourceSearchResult
    {
        public ResourceMetadata[]? Results { get; set; }

        public int Offset { get; set; }

        public int TotalNumResources { get; set; }
    }
}
