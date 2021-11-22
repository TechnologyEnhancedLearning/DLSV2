namespace DigitalLearningSolutions.Data.Models.LearningHubApiClient
{
    public class BulkResourceReferences
    {
        public ResourceReferenceWithReferenceDetails[]? ResourceReferences { get; set; }

        public int[]? UnmatchedReferenceIds { get; set; }
    }
}
