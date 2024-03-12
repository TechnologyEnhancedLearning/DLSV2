namespace DigitalLearningSolutions.Data.Models.External.LearningHubApiClient
{
    public class ResourceReferenceWithResourceDetails
    {
        public int ResourceId { get; set; }

        public int RefId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Catalogue Catalogue { get; set; }  = new Catalogue();

        public string ResourceType { get; set; } = string.Empty;

        public decimal Rating { get; set; }

        public string Link { get; set; } = string.Empty;

        public bool AbsentInLearningHub { get; set; }
    }
}
