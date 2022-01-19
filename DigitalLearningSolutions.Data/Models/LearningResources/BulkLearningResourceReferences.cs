namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class BulkLearningResourceReferences
    {
        public BulkLearningResourceReferences(
            BulkResourceReferences resourceReferences,
            bool sourcedFromFallbackData
        )
        {
            BulkResourceReferences = resourceReferences;
            SourcedFromFallbackData = sourcedFromFallbackData;
        }

        public bool SourcedFromFallbackData { get; set; }

        public BulkResourceReferences BulkResourceReferences { get; set; }
    }
}
