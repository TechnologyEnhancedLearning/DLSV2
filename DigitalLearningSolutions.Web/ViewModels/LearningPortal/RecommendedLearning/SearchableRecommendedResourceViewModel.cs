namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class SearchableRecommendedResourceViewModel
    {
        public SearchableRecommendedResourceViewModel(
            RecommendedResource recommendedResource,
            int selfAssessmentId,
            ReturnPageQuery returnPageQuery
        )
        {
            SelfAssessmentId = selfAssessmentId;
            LearningResourceReferenceId = recommendedResource.LearningResourceReferenceId;
            LearningHubReferenceId = recommendedResource.LearningHubReferenceId;
            ResourceName = recommendedResource.ResourceName;
            ResourceDescription = recommendedResource.ResourceDescription;
            ResourceType = recommendedResource.ResourceType;
            CatalogueName = recommendedResource.CatalogueName;
            ResourceLink = recommendedResource.ResourceLink;
            IsInActionPlan = recommendedResource.IsInActionPlan;
            IsCompleted = recommendedResource.IsCompleted;
            LearningLogItemId = recommendedResource.LearningLogId;
            RecommendationScore = recommendedResource.RecommendationScore;
            ReturnPageQuery = returnPageQuery;
        }

        public int SelfAssessmentId { get; set; }

        public int LearningResourceReferenceId { get; set; }

        public int LearningHubReferenceId { get; set; }

        public string ResourceName { get; set; }

        public string ResourceDescription { get; set; }

        public string ResourceType { get; set; }

        public string CatalogueName { get; set; }

        public string ResourceLink { get; set; }

        public bool IsInActionPlan { get; set; }

        public bool IsCompleted { get; set; }

        public int? LearningLogItemId { get; set; }

        public decimal RecommendationScore { get; set; }

        public ReturnPageQuery ReturnPageQuery { get; set; }

        public string Rating => RecommendationScore >= 100 ? "Essential" :
            RecommendationScore >= 40 ? "Recommended" : "Optional";
    }
}
