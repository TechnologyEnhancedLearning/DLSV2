namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class RecommendedResource
    {
        public RecommendedResource() { }

        public RecommendedResource(
            int resourceReferenceId,
            ResourceReferenceWithResourceDetails resourceReferenceDetails,
            LearningLogItem? incompleteLearningLogItem,
            bool isCompleted
        )
        {
            LearningResourceReferenceId = resourceReferenceId;
            LearningHubReferenceId = resourceReferenceDetails.RefId;
            ResourceName = resourceReferenceDetails.Title;
            ResourceDescription = resourceReferenceDetails.Description;
            ResourceType = resourceReferenceDetails.ResourceType;
            CatalogueName = resourceReferenceDetails.Catalogue.Name;
            ResourceLink = resourceReferenceDetails.Link;
            IsInActionPlan = incompleteLearningLogItem != null;
            IsCompleted = isCompleted;
            LearningLogId = incompleteLearningLogItem?.LearningLogItemId;
            RecommendationScore = 0; // TODO HEEDLS-705 Calculate this score
        }

        public int LearningResourceReferenceId { get; set; }

        public int LearningHubReferenceId { get; set; }

        public string ResourceName { get; set; }

        public string ResourceDescription { get; set; }

        public string ResourceType { get; set; }

        public string CatalogueName { get; set; }

        public string ResourceLink { get; set; }

        public bool IsInActionPlan { get; set; }

        public bool IsCompleted { get; set; }

        public int? LearningLogId { get; set; }

        // TODO HEEDLS-705 Actually populate and use this
        public decimal RecommendationScore { get; set; }
    }
}
