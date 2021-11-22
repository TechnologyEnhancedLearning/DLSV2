namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    public class ResourceReferenceWithReferenceDetails
    {
        public int ResourceId { get; set; }

        public int RefId { get; set; }

        public string? Title { get; set; }

        public Catalogue Catalogue { get; set; }

        public string? ResourceType { get; set; }

        public double Rating { get; set; }

        public string? Link { get; set; }
    }
}
