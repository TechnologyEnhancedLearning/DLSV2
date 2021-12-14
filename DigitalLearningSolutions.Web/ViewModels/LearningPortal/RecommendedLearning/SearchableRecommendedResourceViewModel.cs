namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class SearchableRecommendedResourceViewModel
    {
        public SearchableRecommendedResourceViewModel(RecommendedResource recommendedResource)
        {
            LearningHubReferenceId = recommendedResource.LearningHubReferenceId;
            ResourceName = recommendedResource.ResourceName;
            ResourceDescription = recommendedResource.ResourceDescription;
            ResourceType = recommendedResource.ResourceType;
            CatalogueName = recommendedResource.CatalogueName;
            ResourceLink = recommendedResource.ResourceLink;
            IsInActionPlan = recommendedResource.IsInActionPlan;
            IsCompleted = recommendedResource.IsCompleted;
        }

        public int LearningHubReferenceId { get; set; }

        public string ResourceName { get; set; }

        public string ResourceDescription { get; set; }

        public string ResourceType { get; set; }

        public string CatalogueName { get; set; }

        public string ResourceLink { get; set; }

        public bool IsInActionPlan { get; set; }

        public bool IsCompleted { get; set; }
    }
}
