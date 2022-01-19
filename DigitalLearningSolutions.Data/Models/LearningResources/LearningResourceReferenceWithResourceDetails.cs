namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class LearningResourceReferenceWithResourceDetails
    {
        public LearningResourceReferenceWithResourceDetails(
            ResourceReferenceWithResourceDetails resource,
            bool sourcedFromFallbackData
        )
        {
            ResourceReferenceWithResourceDetails = resource;
            SourcedFromFallbackData = sourcedFromFallbackData;
        }

        public bool SourcedFromFallbackData { get; set; }

        public ResourceReferenceWithResourceDetails ResourceReferenceWithResourceDetails { get; set; }
    }
}
