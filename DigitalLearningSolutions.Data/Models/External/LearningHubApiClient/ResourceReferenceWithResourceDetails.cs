namespace DigitalLearningSolutions.Data.Models.External.LearningHubApiClient
{
    public class ResourceReferenceWithResourceDetails
    {
        public int ResourceId { get; set; }

        public int RefId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Catalogue Catalogue { get; set; }

        public string ResourceType { get; set; }

        public decimal Rating { get; set; }

        public string Link { get; set; }

        public bool AbsentInLearningHub { get; set; }
    }
}
