namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class RecommendedResource : BaseSearchableItem
    {
        public RecommendedResource() { }

        public RecommendedResource(
            int resourceReferenceId,
            ResourceReferenceWithResourceDetails resourceReferenceDetails,
            LearningLogItem? incompleteLearningLogItem,
            bool isCompleted,
            decimal recommendationScore
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
            RecommendationScore = recommendationScore;
        }

        public int LearningResourceReferenceId { get; set; }

        public int LearningHubReferenceId { get; set; }

        public string ResourceName { get; set; } = string.Empty;

        public string ResourceDescription { get; set; } = string.Empty;

        public string ResourceType { get; set; } = string.Empty;

        public string CatalogueName { get; set; } = string.Empty;

        public string ResourceLink { get; set; } = string.Empty;

        public bool IsInActionPlan { get; set; }

        public bool IsCompleted { get; set; }

        public int? LearningLogId { get; set; }

        public decimal RecommendationScore { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? ResourceName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
